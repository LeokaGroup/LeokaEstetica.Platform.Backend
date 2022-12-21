namespace LeokaEstetica.Platform.Models.Entities.Role;

/// <summary>
/// Класс сопоставляется с таблицей ролей модерации Roles.ModerationRoles.
/// </summary>
public class ModerationRoleEntity
{
    public ModerationRoleEntity()
    {
        ModerationUserRoles = new HashSet<ModerationUserRoleEntity>();
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
    /// Список ролей модерации.
    /// </summary>
    public ICollection<ModerationUserRoleEntity> ModerationUserRoles { get; set; }
}