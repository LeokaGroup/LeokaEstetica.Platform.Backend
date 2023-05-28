namespace LeokaEstetica.Platform.Models.Entities.Access;

/// <summary>
/// Класс сопоставляется с теневой таблицей ЧС Access.UserEmailBlackListShadow.
/// </summary>
public class UserEmailBlackListShadowEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long ShadowId { get; set; }

    /// <summary>
    /// Дата создания записи истории.
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Текст события.
    /// </summary>
    public string ActionText { get; set; }

    /// <summary>
    /// Системное название события.
    /// </summary>
    public string ActionSysName { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }
    
    /// <summary>
    /// Почта пользователя.
    /// </summary>
    public string Email { get; set; }
}