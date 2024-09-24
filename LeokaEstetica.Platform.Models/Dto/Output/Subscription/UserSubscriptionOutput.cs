namespace LeokaEstetica.Platform.Models.Dto.Output.Subscription;

/// <summary>
/// Класс выходной модели подписки пользователя.
/// </summary>
public class UserSubscriptionOutput
{
    /// <summary>
    /// PK.
    /// </summary>
    public long SubscriptionId { get; set; }

    /// <summary>
    /// Прзинак активной подписки.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Id тарифа.
    /// </summary>
    public int RuleId { get; set; }

    /// <summary>
    /// Id объекта тарифа.
    /// </summary>
    public int ObjectId { get; set; }

    /// <summary>
    /// Кол-во мес. подписки.
    /// </summary>
    public short? MonthCount { get; set; }
}