using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;

namespace LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;

/// <summary>
/// Класс выходной модели создания заказа в PayMaster.
/// </summary>
public class CreateOrderPayMasterOutput : ICreateOrderOutput, IFrontError
{
    /// <summary>
    /// Id заказа.
    /// </summary>
    public string PaymentId { get; set; }

    /// <summary>
    /// Ссылка на оплату.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure> Errors { get; set; }
}