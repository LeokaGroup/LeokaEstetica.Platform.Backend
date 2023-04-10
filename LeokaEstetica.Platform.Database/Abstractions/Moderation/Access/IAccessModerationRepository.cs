namespace LeokaEstetica.Platform.Database.Abstractions.Moderation.Access;

/// <summary>
/// Абстракция репозитория проверки доступа к модерации.
/// </summary>
public interface IAccessModerationRepository
{
    /// <summary>
    /// Метод проверяет доступ пользователя к КЦ.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Признак результата проверки.</returns>
    Task<bool> CheckAccessUserRoleModerationAsync(long userId);

    /// <summary>
    /// Метод получает хэш пароля.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Хэш пароля.</returns>
    Task<string> GetPasswordHashByEmailAsync(long userId);
}