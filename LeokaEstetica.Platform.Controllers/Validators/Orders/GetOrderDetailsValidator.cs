using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;

namespace LeokaEstetica.Platform.Controllers.Validators.Orders;

/// <summary>
/// Класс валидатора получения деталей заказа.
/// </summary>
public class GetOrderDetailsValidator : AbstractValidator<long>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public GetOrderDetailsValidator()
    {
        RuleFor(p => p)
            .Must(p => p > 0)
            .WithMessage(OrderConst.NOT_VALID_ORDER_ID);
    }
}