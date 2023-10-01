namespace LeokaEstetica.Platform.Models.Entities.Access;

/// <summary>
/// Класс сопоставляется с таблицей ЧС VkUserId пользователей Access.UserVkBlackList.
/// </summary>
public class UserVkBlackListEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long BlackId { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Id пользователя в системе ВКонтакте.
    /// </summary>
    public long VkUserId { get; set; }
}