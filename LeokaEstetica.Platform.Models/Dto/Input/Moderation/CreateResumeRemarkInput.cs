namespace LeokaEstetica.Platform.Models.Dto.Input.Moderation;

/// <summary>
/// Класс списка замечаний анкеты входной модели.
/// </summary>
public class CreateResumeRemarkInput
{
    /// <summary>
    /// Список замечаний.
    /// </summary>
    public IEnumerable<ResumeRemarkInput> ResumesRemarks { get; set; }
}