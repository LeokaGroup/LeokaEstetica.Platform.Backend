namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Resume;

/// <summary>
/// Класс выходной модели списка анкет на модерации.
/// </summary>
public class ResumeModerationResult
{
    /// <summary>
    /// Список анкет.
    /// </summary>
    public IEnumerable<ResumeModerationOutput> Resumes { get; set; }

    /// <summary>
    /// Всего анкет на модерации.
    /// </summary>
    public int Total => Resumes.Count();
}