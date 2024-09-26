namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagementHumanResources;

/// <summary>
/// Класс входной модели календаря.
/// Данные событий календаря.
/// </summary>
public class CalendarInput
{
    /// <summary>
    /// Название события.
    /// </summary>
    public string? EventName { get; set; }

    /// <summary>
    /// Описание события.
    /// </summary>
    public string? EventDescription { get; set; }

    /// <summary>
    /// Место проведения события (адрес или место).
    /// </summary>
    public string? EventLocation { get; set; }
    
    /// <summary>
    /// Дата начала события.
    /// </summary>
    public DateTime EventStartDate { get; set; }
    
    /// <summary>
    /// Дата окончания события.
    /// </summary>
    public DateTime EventEndDate { get; set; }

    /// <summary>
    /// Список участников события.
    /// </summary>
    public List<EventMemberInput>? EventMembers { get; set; }

    /// <summary>
    /// Id пользователя создавшего событие.
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// Статус.
    /// </summary>
    public string? CalendarEventMemberStatus { get; set; }
}