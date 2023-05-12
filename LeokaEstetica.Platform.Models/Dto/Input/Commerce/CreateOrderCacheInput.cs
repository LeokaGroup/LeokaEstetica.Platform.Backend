namespace LeokaEstetica.Platform.Models.Dto.Input.Commerce;

/// <summary>
/// Класс входной модели создания заказа в кэше.
/// </summary>
[Serializable]
public class CreateOrderCacheInput
{
    /// <summary>
    /// Публичный ключ тарифа.
    /// </summary>
    public string PublicId { get; set; }

    /// <summary>
    /// Кол-во месяцев, на которые оформляется подписка на тариф.
    /// </summary>
    public short PaymentMonth { get; set; }
}