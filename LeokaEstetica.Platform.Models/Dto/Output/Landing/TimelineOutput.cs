namespace LeokaEstetica.Platform.Models.Dto.Output.Landing;

/// <summary>
/// Класс выходной модели таймлайна.
/// </summary>
public class TimelineOutput
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