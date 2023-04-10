namespace LeokaEstetica.Platform.Database.Abstractions.Moderation.Access;

/// <summary>
/// Абстракция репозитория проверки доступа к модерации.
/// </summary>
public interface IAccessModerationRepository
{
    /// <summary>
    /// Метод проверяет доступ пользователя к КЦ.
    /// </summary>
    /// <param name="email">Почта.</param>
    /// <param name="password">Пароль.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Признак результата проверки.</returns>
    Task<bool> CheckAccessUserRoleModerationAsync(string email, string password, long userId);
}