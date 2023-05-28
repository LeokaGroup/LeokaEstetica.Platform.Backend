namespace LeokaEstetica.Platform.Models.Entities.Access;

/// <summary>
/// Класс сопоставляется с таблицей ЧС почты пользователей Access.UserEmailBlackList.
/// </summary>
public class UserEmailBlackListEntity
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
    /// Почта пользователя.
    /// </summary>
    public string Email { get; set; }
}