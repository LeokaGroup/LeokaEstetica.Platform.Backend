using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.YandexKassa;

namespace LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;

/// <summary>
/// TODO: Если будет сотрудничество с этой ПС, то прописать тут атрибуты для маппинга.
/// TODO: И также актуализировать список полей по аналогии с другими ПС, но могут быть отличия.
/// Класс выходной модели создания заказа в PayMaster.
/// </summary>
public class CreateOrderPayMasterOutput : ICreateOrderOutput, IFrontError
{
    /// <summary>
    /// Id платежа в ПС.
    /// </summary>
    public string PaymentId { get; set; }
    
    /// <summary>
    /// Ссылка на оплату.
    /// </summary>
    public string ConfirmationUrl { get; set; }

    /// <summary>
    /// Статус платежа в ПС.
    /// </summary>
    public string OrderStatus { get; set; }

    public ConfirmationOutput Confirmation { get; set; }
    public string Type { get; set; }

    /// <summary>
    /// Ссылка в случае успеха оплаты или ошибки оплаты.
    /// </summary>
    public string ReturnUrl { get; set; }

    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure> Errors { get; set; }
}