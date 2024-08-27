namespace LeokaEstetica.Platform.Models.Dto.Output.Roles;

/// <summary>
/// Класс выходной модели компонентных ролей.
/// </summary>
public class ComponentRoleOutput
{
    /// <summary>
    /// Id компонентной роли.
    /// </summary>
    public int ComponentRoleId { get; set; }

    /// <summary>
    /// Название компонентной роли.
    /// </summary>
    public string? ComponentRoleName { get; set; }
    
    /// <summary>
    /// Системное название компонентной роли.
    /// </summary>
    public string? ComponentRoleSysName { get; set; }
}