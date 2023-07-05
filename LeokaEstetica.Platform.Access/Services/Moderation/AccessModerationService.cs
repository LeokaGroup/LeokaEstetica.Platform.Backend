using LeokaEstetica.Platform.Access.Abstractions.Moderation;
using LeokaEstetica.Platform.Access.Helpers;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Access;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Role;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Access.Services.Moderation;

/// <summary>
/// Класс реализует методы сервиса проверки доступа к модерации.
/// </summary>
public class AccessModerationService : IAccessModerationService
{
    private readonly ILogger<AccessModerationService> _logger;
    private readonly IAccessModerationRepository _accessModerationRepository;
    private readonly IUserRepository _userRepository;

    public AccessModerationService(ILogger<AccessModerationService> logger, 
        IAccessModerationRepository accessModerationRepository, 
        IUserRepository userRepository)
    {
        _logger = logger;
        _accessModerationRepository = accessModerationRepository;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Метод проверяет доступ пользователя к КЦ.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные выходной модели.</returns>
    public async Task<ModerationRoleOutput> CheckUserRoleModerationAsync(string account)
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
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}