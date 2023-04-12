using LeokaEstetica.Platform.Models.Entities.Moderation;

namespace LeokaEstetica.Platform.Models.Entities.Role;

/// <summary>
/// Класс сопоставляется с таблицей ролей модерации Roles.ModerationRoles.
/// </summary>
public class ModerationRoleEntity
{
    public ModerationRoleEntity()
    {
        ModerationUsers = new HashSet<ModerationUserEntity>();
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
    /// Список пользователей, имеющих доступ к КЦ.
    /// </summary>
    public ICollection<ModerationUserEntity> ModerationUsers { get; set; }
}