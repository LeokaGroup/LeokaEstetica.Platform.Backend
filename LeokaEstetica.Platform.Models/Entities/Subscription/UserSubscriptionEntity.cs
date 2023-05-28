namespace LeokaEstetica.Platform.Models.Entities.Subscription;

/// <summary>
/// Класс сопоставляется с таблицей подписок пользователя.
/// </summary>
public class UserSubscriptionEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long UserSubscriptionId { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Признак активности подписки пользователя.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Кол-во мес. на которые приобретена подписка.
    /// </summary>
    public short MonthCount { get; set; }

    /// <summary>
    /// Id подписки.
    /// </summary>
    public long SubscriptionId { get; set; }
}