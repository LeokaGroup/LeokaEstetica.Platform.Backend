namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Resume;

/// <summary>
/// Класс списка выходной модели анкет, ожидающих проверки замечаний.
/// </summary>
public class AwaitingCorrectionResumeResult
{
    /// <summary>
    /// Список анкет.
    /// </summary>
    public IEnumerable<ResumeRemarkOutput> AwaitingCorrectionResumes { get; set; }

    /// <summary>
    /// Кол-во.
    /// </summary>
    public int Total => AwaitingCorrectionResumes.Count();
}