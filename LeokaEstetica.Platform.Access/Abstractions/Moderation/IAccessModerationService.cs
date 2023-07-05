using LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Role;

namespace LeokaEstetica.Platform.Access.Abstractions.Moderation;

/// <summary>
/// Абстракция сервиса проверки доступа к модерации.
/// </summary>
public interface IAccessModerationService
{
    /// <summary>
    /// Метод проверяет доступ пользователя к КЦ.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные выходной модели.</returns>
    Task<ModerationRoleOutput> CheckUserRoleModerationAsync(string account);
}