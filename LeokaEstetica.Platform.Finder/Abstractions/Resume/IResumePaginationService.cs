using LeokaEstetica.Platform.Models.Dto.Output.Resume;

namespace LeokaEstetica.Platform.Finder.Abstractions.Resume;

/// <summary>
/// Абстракция сервиса пагинации резюме.
/// </summary>
public interface IResumePaginationService
{
    /// <summary>
    /// Метод пагинации резюме.
    /// </summary>
    /// <param name="page">Номер страницы.</param>
    /// <returns>Список резюме.</returns>
    Task<PaginationResumeOutput> GetResumesPaginationAsync(int page);
}