using LeokaEstetica.Platform.Moderation.Models.Dto.Output.Role;

namespace LeokaEstetica.Platform.Access.Abstractions.Moderation;

/// <summary>
/// Абстракция сервиса проверки доступа к модерации.
/// </summary>
public interface IAccessModerationService
{
    /// <summary>
    /// Метод проверяет, имеет ли пользователь роль, которая дает доступ к модерации.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные выходной модели.</returns>
    Task<ModerationRoleOutput> CheckUserRoleModerationAsync(string account);
}