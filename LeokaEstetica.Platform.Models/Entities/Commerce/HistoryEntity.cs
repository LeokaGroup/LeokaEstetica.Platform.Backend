namespace LeokaEstetica.Platform.Models.Entities.Commerce;

/// <summary>
/// Класс сопоставляется с таблицей транзакций по заказам пользователя Commerce.OrderTransactionsShadow.
/// </summary>
public class HistoryEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long ShadowId { get; set; }

    /// <summary>
    /// Дата транзакции.
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
    /// Id заказа.
    /// </summary>
    public long OrderId { get; set; }

    /// <summary>
    /// Название статуса.
    /// </summary>
    public string StatusName { get; set; }
}