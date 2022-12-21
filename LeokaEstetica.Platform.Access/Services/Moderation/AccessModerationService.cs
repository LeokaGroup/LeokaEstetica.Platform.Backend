using LeokaEstetica.Platform.Access.Abstractions;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Moderation.Models.Dto.Output;

namespace LeokaEstetica.Platform.Access.Services.Moderation;

/// <summary>
/// Класс реализует методы сервиса проверки доступа к модерации.
/// </summary>
public sealed class AccessModerationService : IAccessModerationService
{
    private readonly ILogService _logService;

    public AccessModerationService(ILogService logService)
    {
        _logService = logService;
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
            
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex,
                $"У пользователя {account} нет доступа к модерации. Возможно отсутствует роль либо она отключена.");
            throw;
        }
    }
}