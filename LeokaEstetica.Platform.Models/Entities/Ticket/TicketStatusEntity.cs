namespace LeokaEstetica.Platform.Models.Entities.Ticket;

/// <summary>
/// Класс сопоставляется с таблицей статусов тикетов.
/// </summary>
public class TicketStatusEntity
{
    public TicketStatusEntity(string statusName, string statusSysName)
    {
		StatusName = statusName;
        StatusSysName = statusSysName;
	}
    /// <summary>
    /// PK.
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Название статуса.
    /// </summary>
    public string StatusName { get; set; }
    
    /// <summary>
    /// TODO: Будет енамкой.
    /// Системное название статуса.
    /// </summary>
    public string StatusSysName { get; set; }
}