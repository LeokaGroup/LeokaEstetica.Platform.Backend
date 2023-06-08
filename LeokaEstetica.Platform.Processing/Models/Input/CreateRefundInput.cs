namespace LeokaEstetica.Platform.Processing.Models.Input;

/// <summary>
/// Класс входной модели создания возврата для запроса в ПС.
/// </summary>
[Serializable]
public class CreateRefundInput
{
    /// <summary>
    /// Id платежа в ПС.
    /// </summary>
    public string PaymentId { get; set; }

    /// <summary>
    /// Сумма возврата.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Валюта.
    /// </summary>
    public string Currency { get; set; }
}