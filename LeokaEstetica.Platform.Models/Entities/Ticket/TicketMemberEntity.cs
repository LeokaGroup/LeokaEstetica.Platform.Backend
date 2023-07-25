using LeokaEstetica.Platform.Models.Entities.User;

namespace LeokaEstetica.Platform.Models.Entities.Ticket;

/// <summary>
/// Класс сопоставляется с таблицей участников тикета Communications.TicketMembers.
/// </summary>
public class TicketMemberEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long MemberId { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Дата присоединения участника тикета.
    /// </summary>
    public DateTime Joined { get; set; }

    /// <summary>
    /// Id тикета.
    /// </summary>
    public long TicketId { get; set; }

    /// <summary>
    /// FK на основную информацию о тикете.
    /// </summary>
    public MainInfoTicketEntity MainInfoTicket { get; set; }

    /// <summary>
    /// FK на пользователя.
    /// </summary>
    public UserEntity User { get; set; }
}