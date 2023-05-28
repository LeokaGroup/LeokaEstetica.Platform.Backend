using LeokaEstetica.Platform.Models.Dto.Base.Moderation.Output;

namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;

/// <summary>
/// Класс выходной модели замечания проекта.
/// </summary>
public class ProjectRemarkOutput : BaseRemarkOutput
{
    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }
}