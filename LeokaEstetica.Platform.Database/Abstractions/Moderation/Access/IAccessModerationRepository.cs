namespace LeokaEstetica.Platform.Database.Abstractions.Moderation.Access;

/// <summary>
/// Абстракция репозитория проверки доступа к модерации.
/// </summary>
public interface IAccessModerationRepository
{
    /// <summary>
    /// Метод првоеряет доступ пользователя к модерации.
    /// Идет проверка на наличие допустимой роли.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Признак результата проверки.</returns>
    Task<bool> CheckAccessUserRoleModerationAsync(long userId);
}