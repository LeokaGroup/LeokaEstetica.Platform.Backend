using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;

namespace LeokaEstetica.Platform.Controllers.Validators.Commerce;

/// <summary>
/// Класс валидатора создания заказа.
/// </summary>
public class CreateOrderValidator : AbstractValidator<CreateOrderInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public CreateOrderValidator()
    {
        RuleFor(p => p.CreateOrderRequest.FareRuleId)
            .Must(p => p > 0)
            .WithMessage(CommerceConst.NOT_VALID_FARE_RULE_ID);
    }
}