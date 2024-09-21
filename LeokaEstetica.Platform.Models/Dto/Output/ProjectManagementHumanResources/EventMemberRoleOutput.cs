namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagementHumanResources;

/// <summary>
/// Класс выходной модели ролей участника события.
/// </summary>
public class EventMemberRoleOutput
{
    /// <summary>
    /// Id роли.
    /// </summary>
    public long RoleId { get; set; }

    /// <summary>
    /// Id события календаря.
    /// </summary>
    public long EventId { get; set; }

    /// <summary>
    /// Название роли.
    /// </summary>
    public string? RoleName { get; set; }
    
    /// <summary>
    /// Системное название роли.
    /// </summary>
    public string? RoleSysName { get; set; }

    /// <summary>
    /// Id участника события.
    /// </summary>
    public long EventMemberId { get; set; }
}