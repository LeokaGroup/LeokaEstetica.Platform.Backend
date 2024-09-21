using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagementHumanResources;

/// <summary>
/// Класс выходной модели календаря.
/// Данные событий календаря.
/// </summary>
public class CalendarOutput
{
    /// <summary>
    /// Id события.
    /// PK.
    /// </summary>
    public long EventId { get; set; }

    /// <summary>
    /// Название события.
    /// </summary>
    [JsonProperty("title")]
    public string? EventName { get; set; }

    /// <summary>
    /// Описание события.
    /// </summary>
    public string? EventDescription { get; set; }

    /// <summary>
    /// Id пользователя, который создал событие.
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// Дата начала события.
    /// </summary>
    [JsonProperty("start")]
    public DateTime EventStartDate { get; set; }
    
    /// <summary>
    /// Дата окончания события.
    /// </summary>
    [JsonProperty("end")]
    public DateTime EventEndDate { get; set; }

    /// <summary>
    /// Место проведения события (адрес или место).
    /// </summary>
    public string? EventLocation { get; set; }

    /// <summary>
    /// Список участников события.
    /// </summary>
    public List<EventMemberOutput>? EventMembers { get; set; }

    /// <summary>
    /// Список ролей участника события.
    /// </summary>
    public List<EventMemberRoleOutput>? EventMemberRoles { get; set; }
}