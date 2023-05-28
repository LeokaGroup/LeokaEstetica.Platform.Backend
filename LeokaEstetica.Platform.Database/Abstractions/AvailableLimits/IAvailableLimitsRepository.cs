namespace LeokaEstetica.Platform.Database.Abstractions.AvailableLimits;

/// <summary>
/// Абстракция репозитория проверки лимитов.
/// </summary>
public interface IAvailableLimitsRepository
{
    /// <summary>
    /// Метод получает кол-во уже созданных проектов пользователем.
    /// Считаем только активные проекты (т.е. которые находятся в каталоге).
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Кол-во созданных пользователем проектов.</returns>
    Task<int> CheckAvailableCreateProjectAsync(long userId);
    
    /// <summary>
    /// Метод получает кол-во уже созданных вакансий пользователем.
    /// Считаем только активные вакансии (т.е. которые находятся в каталоге).
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Кол-во созданных пользователем вакансий.</returns>
    Task<int> CheckAvailableCreateVacancyAsync(long userId);
}