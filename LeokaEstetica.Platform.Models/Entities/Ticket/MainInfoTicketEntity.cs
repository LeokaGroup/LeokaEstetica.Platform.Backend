namespace LeokaEstetica.Platform.Models.Entities.Ticket;

/// <summary>
/// Класс сопоставляется с таблицей основной информации о тикете Communications.MainInfoTickets.
/// </summary>
public class MainInfoTicketEntity
{
    public MainInfoTicketEntity(string ticketName)
    {
        TicketName = ticketName;
        TicketMembers = new HashSet<TicketMemberEntity>();
        TicketMessages = new HashSet<TicketMessageEntity>();
    }

    /// <summary>
    /// PK.
    /// </summary>
    public long TicketId { get; set; }

    /// <summary>
    /// Название тикета.
    /// </summary>
    public string TicketName { get; set; }

    /// <summary>
    /// Дата создания тикета.
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Id статуса тикета.
    /// </summary>
    public short TicketStatusId { get; set; }

    /// <summary>
    /// Id файла тикета.
    /// </summary>
    public long? TicketFileId { get; set; }

    /// <summary>
    /// Список участников тикета.
    /// </summary>
    public ICollection<TicketMemberEntity> TicketMembers { get; set; }

    /// <summary>
    /// Сообщения тикета.
    /// </summary>
    public ICollection<TicketMessageEntity> TicketMessages { get; set; }
}