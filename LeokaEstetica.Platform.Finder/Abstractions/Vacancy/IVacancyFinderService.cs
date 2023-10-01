using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

namespace LeokaEstetica.Platform.Finder.Abstractions.Vacancy;

/// <summary>
/// Абстракция поискового сервиса вакансий.
/// </summary>
public interface IVacancyFinderService
{
    /// <summary>
    /// Метод находит вакансии по поисковому запросу.
    /// </summary>
    /// <param name="searchText">Строка поиска.</param>
    /// <returns>Список вакансий соответствующие поисковому запросу.</returns>
    Task<CatalogVacancyResultOutput> SearchVacanciesAsync(string searchText);
}