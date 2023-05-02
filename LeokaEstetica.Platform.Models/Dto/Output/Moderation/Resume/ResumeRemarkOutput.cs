using LeokaEstetica.Platform.Models.Dto.Base.Moderation.Output;

namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Resume;

/// <summary>
/// Класс выходной модели замечания анкет.
/// </summary>
public class ResumeRemarkOutput : BaseRemarkOutput
{
    /// <summary>
    /// Id анкеты.
    /// </summary>
    public long ProfileInfoId { get; set; }
}