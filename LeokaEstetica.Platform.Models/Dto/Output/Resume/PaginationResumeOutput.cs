using LeokaEstetica.Platform.Models.Dto.Output.Pagination;

namespace LeokaEstetica.Platform.Models.Dto.Output.Resume;

/// <summary>
/// Класс выходной модели пагинации резюме.
/// </summary>
public class PaginationResumeOutput : BasePaginationInfo
{
    /// <summary>
    /// Список резюме.
    /// </summary>
    public List<ResumeOutput> Resumes { get; set; }

    /// <summary>
    /// Кол-во всего.
    /// </summary>
    public int Total => Resumes.Count;
}