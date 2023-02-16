namespace LeokaEstetica.Platform.Moderation.Models.Dto.Input.Resume;

/// <summary>
/// Класс входной модели отклонения анкеты.
/// </summary>
public class RejectResumeInput
{
    /// <summary>
    /// Id анкеты.
    /// </summary>
    public long ProfileInfoId { get; set; }
}