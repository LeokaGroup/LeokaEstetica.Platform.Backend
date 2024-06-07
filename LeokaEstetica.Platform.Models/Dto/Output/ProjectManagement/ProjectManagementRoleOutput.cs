namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;

/// <summary>
/// Класс выходной модели ролей модуля УП.
/// </summary>
public class ProjectManagementRoleOutput
{
    /// <summary>
    /// Id компании.
    /// </summary>
    public long OrganizationId { get; set; }

    /// <summary>
    /// Id участника компании.
    /// </summary>
    public long OrganizationMemberId { get; set; }

    /// <summary>
    /// Название роли.
    /// </summary>
    public string? RoleName { get; set; }

    /// <summary>
    /// Системное название роли.
    /// </summary>
    public string? RoleSysName { get; set; }

    /// <summary>
    /// Признак активной роли.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Признак активной роли у участника проекта компании.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Id проекта компании.
    /// </summary>
    public long? ProjectId { get; set; }

    /// <summary>
    /// Email пользователя.
    /// </summary>
    public string? Email { get; set; }
}