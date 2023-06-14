using LeokaEstetica.Platform.Models.Dto.Base.Commerce.PayMaster;

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
    /// Цена.
    /// </summary>
    public Amount Amount { get; set; }
}