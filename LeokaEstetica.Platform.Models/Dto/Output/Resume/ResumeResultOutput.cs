namespace LeokaEstetica.Platform.Models.Dto.Output.Resume;

/// <summary>
/// Класс выходного списка с резюме.
/// </summary>
public class ResumeResultOutput
{
    /// <summary>
    /// Список резюме.
    /// </summary>
    public IEnumerable<ResumeOutput> CatalogResumes { get; set; }

    /// <summary>
    /// Кол-во.
    /// </summary>
    public int Total => CatalogResumes.Count();
}