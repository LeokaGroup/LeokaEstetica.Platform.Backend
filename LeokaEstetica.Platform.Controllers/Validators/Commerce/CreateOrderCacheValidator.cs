using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce;

namespace LeokaEstetica.Platform.Controllers.Validators.Commerce;

/// <summary>
/// Класс валидатора создания заказа в кэше.
/// </summary>
public class CreateOrderCacheValidator : AbstractValidator<CreateOrderCacheInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public CreateOrderCacheValidator()
    {
        RuleFor(p => p.PublicId)
            .NotNull()
            .WithMessage(CommerceConst.EMPTY_PUBLIC_ID);
        
        RuleFor(p => p.PaymentMonth)
            .Must(p => p is > 0 and <= 12)
            .WithMessage(CommerceConst.NOT_VALID_MONTH);
    }
}