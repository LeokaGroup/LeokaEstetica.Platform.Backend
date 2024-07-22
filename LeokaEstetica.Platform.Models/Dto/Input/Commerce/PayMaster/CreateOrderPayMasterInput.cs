using LeokaEstetica.Platform.Models.Dto.Input.Base;

namespace LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;

/// <summary>
/// Класс входной модели создания заказа.
/// </summary>
[Serializable]
public class CreateOrderPayMasterInput : ICreateOrderInput
{
    /// <summary>
    /// Модель запроса к ПС для создания заказа.
    /// </summary>
    public CreateOrderPayMasterRequest? CreateOrderRequest { get; set; }

    /// <summary>
    /// Публичный ключ тарифа.
    /// </summary>
    public Guid PublicId { get; set; }
}