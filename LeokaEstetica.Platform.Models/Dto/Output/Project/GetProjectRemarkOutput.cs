using LeokaEstetica.Platform.Models.Dto.Base.Moderation.Output;

namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели замечаний проекта.
/// </summary>
public class GetProjectRemarkOutput : BaseGetRemarkOutput
{
    public GetProjectRemarkOutput()
    {
        Severity = "warn";
    }
}