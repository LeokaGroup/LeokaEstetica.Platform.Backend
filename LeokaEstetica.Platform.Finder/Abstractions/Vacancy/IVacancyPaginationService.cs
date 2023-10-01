using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

namespace LeokaEstetica.Platform.Finder.Abstractions.Vacancy;

/// <summary>
/// Абстракция сервиса пагинации вакансий.
/// </summary>
public interface IVacancyPaginationService
{
    /// <summary>
    /// Метод пагинации вакансий.
    /// </summary>
    /// <param name="page">Номер страницы.</param>
    /// <returns>Список вакансий.</returns>
    Task<PaginationVacancyOutput> GetVacanciesPaginationAsync(int page);
}