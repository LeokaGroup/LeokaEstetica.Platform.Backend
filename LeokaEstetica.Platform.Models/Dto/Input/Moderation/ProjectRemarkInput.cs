using LeokaEstetica.Platform.Models.Dto.Base.Moderation.Input;

namespace LeokaEstetica.Platform.Models.Dto.Input.Moderation;

/// <summary>
/// Класс входной модели замечаний проекта.
/// </summary>
public class ProjectRemarkInput : BaseRemarkInput
{
    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }
}