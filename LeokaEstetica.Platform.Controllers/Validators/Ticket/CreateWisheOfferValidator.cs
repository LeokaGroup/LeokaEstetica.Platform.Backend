using System.Text.RegularExpressions;
using FluentValidation;
using LeokaEstetica.Platform.Models.Dto.Input.Ticket;
using LeokaEstetica.Platform.Services.Consts;

namespace LeokaEstetica.Platform.Controllers.Validators.Ticket;

/// <summary>
/// Класс валидатора создания предложения/предложения.
/// </summary>
public class CreateWisheOfferValidator : AbstractValidator<WisheOfferInput>
{
    /// <summary>
    /// Русское название.
    /// </summary>
    private readonly string _wisheOfferText = "Текст обращения";

    /// <summary>
    /// Русское название.
    /// </summary>
    private readonly string _contactEmail = "Email";
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public CreateWisheOfferValidator()
    {
        RuleFor(p => p.WisheOfferText)
            .NotNull()
            .WithMessage(ValidationConsts.EMPTY_WISHE_OFFER_TEXT)
            .WithName(_wisheOfferText)
            .NotEmpty()
            .WithMessage(ValidationConsts.EMPTY_WISHE_OFFER_TEXT)
            .WithName(_wisheOfferText);
        
        RuleFor(p => p.ContactEmail)
            .NotNull()
            .WithMessage(ValidationConsts.NOT_VALID_EMAIL_ERROR)
            .WithName(_contactEmail)
            .NotEmpty()
            .WithMessage(ValidationConsts.NOT_VALID_EMAIL_ERROR)
            .Matches("[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}", RegexOptions.IgnoreCase)
            .WithName(_contactEmail);
    }
}