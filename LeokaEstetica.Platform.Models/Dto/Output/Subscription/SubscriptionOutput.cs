namespace LeokaEstetica.Platform.Models.Dto.Output.Subscription;

/// <summary>
/// Класс выходной модели подписок.
/// </summary>
public class SubscriptionOutput
{
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
    public string? SubscriptionType { get; set; }

    /// <summary>
    /// Признак активной подписки пользователя (вычисляемое поле).
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Название подписки.
    /// </summary>
    public string? SubscriptionName { get; set; }
}