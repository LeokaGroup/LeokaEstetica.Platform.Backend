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
    /// <param name="fareRuleName">Название тарифа.</param>
    /// <returns>Признак доступости.</returns>
    Task<bool> CheckAvailableCreateProjectAsync(long userId, string fareRuleName);
    
    /// <summary>
    /// Метод проверяет, доступны ли пользователю для создания вакансии в зависимости от подписки. 
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="fareRuleName">Название тарифа.</param>
    /// <returns>Признак доступости.</returns>
    Task<bool> CheckAvailableCreateVacancyAsync(long userId, string fareRuleName);
}