using LeokaEstetica.Platform.Models.Dto.Base.Moderation.Output;

namespace LeokaEstetica.Platform.Models.Dto.Output.Resume;

/// <summary>
/// Класс выходной модели замечаний анкет.
/// </summary>
public class GetResumeRemarkOutput : BaseGetRemarkOutput
{
    public GetResumeRemarkOutput()
    {
        Severity = "warn";
    }
}