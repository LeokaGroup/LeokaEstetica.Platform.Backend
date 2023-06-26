using LeokaEstetica.Platform.Models.Entities.User;

namespace LeokaEstetica.Platform.Models.Entities.Ticket;

/// <summary>
/// Класс сопоставляется с таблицей ролей пользователя тикетов.
/// </summary>
public class UserTicketRoleEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long UserRoleId { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Id роли тикета.
    /// </summary>
    public int RoleId { get; set; }
    
    /// <summary>
    /// FK на пользователя.
    /// </summary>
    public UserEntity User { get; set; }

    /// <summary>
    /// FK на роль тикета.
    /// </summary>
    public TicketRoleEntity TicketRole { get; set; }
}