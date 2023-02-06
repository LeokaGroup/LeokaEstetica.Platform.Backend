namespace LeokaEstetica.Platform.Access.Abstractions.AvailableLimits;

/// <summary>
/// Абстракция сервиса проверки лимитов.
/// </summary>
public interface IAvailableLimitsService
{
    /// <summary>
    /// Метод проверяет, доступны ли пользователю для создания проекты в зависимости от подписки. 
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Признак доступости.</returns>
    Task<bool> CheckAvailableCreateProjectAsync(long userId);
}