using LeokaEstetica.Platform.Models.Entities.User;

namespace LeokaEstetica.Platform.Models.Entities.Ticket;

/// <summary>
/// Класс сопоставляется с таблицей сообщений тикетов Communications.TicketMessages.
/// </summary>
public class TicketMessageEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long MessageId { get; set; }

    /// <summary>
    /// Id тикета.
    /// </summary>
    public long TicketId { get; set; }

    /// <summary>
    /// Сообщение.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Дата создания сообщения.
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Признак сообщения текущего пользователя.
    /// </summary>
    public bool IsMyMessage { get; set; }
    
    /// <summary>
    /// FK на пользователя.
    /// </summary>
    public UserEntity User { get; set; }
    
    /// <summary>
    /// FK на основную информацию о тикете.
    /// </summary>
    public MainInfoTicketEntity MainInfoTicket { get; set; }
}