namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели обновления ролей.
/// </summary>
public class UpdateRoleInput
{
    /// <summary>
    /// Список ролей для обновления.
    /// </summary>
    public IEnumerable<ProjectManagementRoleInput>? Roles { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }
    
    /// <summary>
    /// Id компании.
    /// </summary>
    public long CompanyId { get; set; }
}