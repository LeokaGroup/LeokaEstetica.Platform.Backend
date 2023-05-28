using LeokaEstetica.Platform.Models.Entities.User;

namespace LeokaEstetica.Platform.Models.Entities.Role;

/// <summary>
/// Класс сопоставляется с таблицей ролей модерации пользователя Roles.ModerationUserRoles.
/// </summary>
public class ModerationUserRoleEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int UserRoleId { get; set; }

    /// <summary>
    /// Id роли.
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Признак активности роли у пользователя.
    /// С помощью него можем отключить роль пользователю.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// FK.
    /// </summary>
    public ModerationRoleEntity ModerationRole { get; set; }

    /// <summary>
    /// FK.
    /// </summary>
    public UserEntity User { get; set; }
}