using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;

/// <summary>
/// Класс выходной модели создания заказа.
/// </summary>
public class CreateOrderOutput : IFrontError
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