namespace LeokaEstetica.Platform.Models.Entities.Ticket;

/// <summary>
/// Класс сопоставляется с таблицей ролей тикета.
/// </summary>
public class TicketRoleEntity
{
    public TicketRoleEntity(string roleName, string roleSysName)
    {
        RoleName = roleName;
        RoleSysName = roleSysName;
        UserTicketRoles = new HashSet<UserTicketRoleEntity>();
    }

    /// <summary>
    /// PK.
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// Название роли.
    /// </summary>
    public string RoleName { get; set; }

    /// <summary>
    /// Системное название роли.
    /// </summary>
    public string RoleSysName { get; set; }

    /// <summary>
    /// Роли пользователей тикета.
    /// </summary>
    public ICollection<UserTicketRoleEntity> UserTicketRoles { get; set; }
}