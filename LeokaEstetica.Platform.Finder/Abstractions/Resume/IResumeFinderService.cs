using LeokaEstetica.Platform.Models.Dto.Output.Resume;

namespace LeokaEstetica.Platform.Finder.Abstractions.Resume;

/// <summary>
/// Абстракция поискового сервиса резюме.
/// </summary>
public interface IResumeFinderService
{
    /// <summary>
    /// Метод находит резюме по поисковому запросу.
    /// </summary>
    /// <param name="searchText">Поисковая строка.</param>
    /// <returns>Список резюме после поиска.</returns>
    Task<ResumeResultOutput> SearchResumesAsync(string searchText);
}