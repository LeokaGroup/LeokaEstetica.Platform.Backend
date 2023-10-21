using LeokaEstetica.Platform.Models.Dto.Input.Base;

namespace LeokaEstetica.Platform.Models.Dto.Input.Commerce.YandexKassa;

/// <summary>
/// Класс входной модели создания заказа в ЮKassa.
/// </summary>
[Serializable]
public class CreateOrderYandexKassaInput : ICreateOrderInput
{
    /// <summary>
    /// Модель запроса к ПС для создания заказа.
    /// </summary>
    public CreateOrderYandexKassaRequest CreateOrderRequest { get; set; }

    /// <summary>
    /// Публичный ключ тарифа.
    /// </summary>
    public Guid PublicId { get; set; }
}