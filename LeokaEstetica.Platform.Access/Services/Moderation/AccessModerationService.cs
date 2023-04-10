using FluentValidation.Results;
using LeokaEstetica.Platform.Access.Abstractions.Moderation;
using LeokaEstetica.Platform.Access.Helpers;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Access;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Role;
using LeokaEstetica.Platform.Models.Dto.Output.User;

namespace LeokaEstetica.Platform.Access.Services.Moderation;

/// <summary>
/// Класс реализует методы сервиса проверки доступа к модерации.
/// </summary>
public class AccessModerationService : IAccessModerationService
{
    private readonly ILogService _logService;
    private readonly IAccessModerationRepository _accessModerationRepository;
    private readonly IUserRepository _userRepository;

    public AccessModerationService(ILogService logService, 
        IAccessModerationRepository accessModerationRepository, 
        IUserRepository userRepository)
    {
        _logService = logService;
        _accessModerationRepository = accessModerationRepository;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Метод проверяет доступ пользователя к КЦ.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <param name="password">Пароль.</param>
    /// <returns>Данные выходной модели.</returns>
    public async Task<ModerationRoleOutput> CheckUserRoleModerationAsync(string account, string password)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                throw new NotFoundUserIdByAccountException(account);
            }
            
            var passwordHash = await _accessModerationRepository.GetPasswordHashByEmailAsync(userId);

            if (passwordHash is null)
            {
                throw new InvalidOperationException("Хэш пароль не удалось получить для пользователя. " +
                                                    $"UserId: {userId}." +
                                                    $"Account: {account}");
            }

            var checkPassword = HashHelper.VerifyHashedPassword(passwordHash, password);

            if (!checkPassword)
            {
                throw new UnauthorizedAccessException("Пользователь не прошел проверку по паролю.");
            }

            var isRole = await _accessModerationRepository.CheckAccessUserRoleModerationAsync(userId);

            // Если нет нужной роли, не пускаем к модерации.
            if (!isRole)
            {
                throw new InvalidOperationException($"У пользователя нет прав на доступ к КЦ. UserId: {userId}");
            }

            var result = new ModerationRoleOutput { AccessModeration = true };

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }
}