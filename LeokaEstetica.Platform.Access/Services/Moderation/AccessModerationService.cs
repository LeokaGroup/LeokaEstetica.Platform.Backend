using LeokaEstetica.Platform.Access.Abstractions.Moderation;
using LeokaEstetica.Platform.Access.Exceptions;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Access;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Role;

namespace LeokaEstetica.Platform.Access.Services.Moderation;

/// <summary>
/// Класс реализует методы сервиса проверки доступа к модерации.
/// </summary>
public sealed class AccessModerationService : IAccessModerationService
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
    /// Метод проверяет, имеет ли пользователь роль, которая дает доступ к модерации.
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

            var isRole = await _accessModerationRepository.CheckAccessUserRoleModerationAsync(userId);

            // Если нет нужной роли, не пускаем к модерации.
            if (!isRole)
            {
                throw new NotAvailableAccessModerationRoleException(account);
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