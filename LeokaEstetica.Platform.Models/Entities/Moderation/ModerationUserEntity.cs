using LeokaEstetica.Platform.Models.Entities.Role;
using LeokaEstetica.Platform.Models.Entities.User;

namespace LeokaEstetica.Platform.Models.Entities.Moderation;

/// <summary>
/// Класс сопоставляется с таблицей пользователей, которым разрешен доступ к КЦ Moderation.Users.
/// </summary>
public class ModerationUserEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// FK на пользователя dbo.Users.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// FK.
    /// </summary>
    public UserEntity User { get; set; }

    /// <summary>
    /// Дата добавления пользователя.
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// FK на роль пользователя Roles.ModerationRoles.
    /// </summary>
    public int UserRoleId { get; set; }

    /// <summary>
    /// FK.
    /// </summary>
    public ModerationRoleEntity ModerationRole { get; set; }

    /// <summary>
    /// Хэш пароля.
    /// </summary>
    public string PasswordHash { get; set; }
}