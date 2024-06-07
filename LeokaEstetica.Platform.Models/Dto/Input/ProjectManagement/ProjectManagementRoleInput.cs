namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели ролей модуля УП.
/// </summary>
public class ProjectManagementRoleInput
{
    /// <summary>
    /// Id роли.
    /// </summary>
    public long RoleId { get; set; }
    
    /// <summary>
    /// Признак активной роли у участника проекта компании.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }
}