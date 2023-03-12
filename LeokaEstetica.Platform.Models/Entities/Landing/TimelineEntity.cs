namespace LeokaEstetica.Platform.Models.Entities.Landing;

/// <summary>
/// Класс сопоставляется с таблицей таймлайнов dbo.Timelines.
/// </summary>
public class TimelineEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long TimelineId { get; set; }

    /// <summary>
    /// Название таймлайна.
    /// </summary>
    public string TimelineTitle { get; set; }

    /// <summary>
    /// Описание.
    /// </summary>
    public string TimelineText { get; set; }

    /// <summary>
    /// Системное название типа таймлайна.
    /// </summary>
    public string TimelineSysType { get; set; }
    
    /// <summary>
    /// Название типа таймлайна.
    /// </summary>
    public string TimelineTypeName { get; set; }
}