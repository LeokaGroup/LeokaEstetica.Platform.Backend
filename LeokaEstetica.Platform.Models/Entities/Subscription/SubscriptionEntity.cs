namespace LeokaEstetica.Platform.Models.Entities.Subscription;

/// <summary>
/// Класс сопоставляется с таблицей подписок Subscriptions.Subscriptions.
/// </summary>
public class SubscriptionEntity
{
    public SubscriptionEntity(string subscriptionType)
    {
        SubscriptionType = subscriptionType;
    }
    /// <summary>
    /// PK.
    /// </summary>
    public long SubscriptionId { get; set; }

    /// <summary>
    /// Id объекта подписки.
    /// </summary>
    public long ObjectId { get; set; }

    /// <summary>
    /// Признак доступности подписки.
    /// </summary>
    public bool IsLatter { get; set; }

    /// <summary>
    /// Тип подписки.
    /// </summary>
    public string SubscriptionType { get; set; }
}