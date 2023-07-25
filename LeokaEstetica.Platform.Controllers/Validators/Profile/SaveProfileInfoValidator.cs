using System.Text.RegularExpressions;
using FluentValidation;
using LeokaEstetica.Platform.Models.Dto.Input.Profile;
using LeokaEstetica.Platform.Services.Consts;

namespace LeokaEstetica.Platform.Controllers.Validators.Profile;

/// <summary>
/// Класс валидатора сохранения информации профиля.
/// </summary>
public class SaveProfileInfoValidator : AbstractValidator<ProfileInfoInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public SaveProfileInfoValidator()
    {
        RuleFor(p => p.FirstName)
            .NotNull()
            .WithMessage(ValidationConsts.EMPTY_FIRST_NAME_ERROR)
            .NotEmpty()
            .WithMessage(ValidationConsts.EMPTY_FIRST_NAME_ERROR);
        
        RuleFor(p => p.LastName)
            .NotNull()
            .WithMessage(ValidationConsts.EMPTY_LAST_NAME_ERROR)
            .NotEmpty()
            .WithMessage(ValidationConsts.EMPTY_LAST_NAME_ERROR);
        
        RuleFor(p => p.Aboutme)
            .NotNull()
            .WithMessage(ValidationConsts.EMPTY_ABOUTME_ERROR)
            .NotEmpty()
            .WithMessage(ValidationConsts.EMPTY_ABOUTME_ERROR);

        RuleFor(p => p.Email)
            .NotNull()
            .WithMessage(ValidationConsts.EMPTY_EMAIL_ERROR)
            .NotEmpty()
            .WithMessage(ValidationConsts.EMPTY_EMAIL_ERROR)
            .Matches("[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}", RegexOptions.IgnoreCase);
        
        RuleFor(p => p.PhoneNumber)
            .NotNull()
            .WithMessage(ValidationConsts.EMPTY_PHONE_NUMBER_ERROR)
            .NotEmpty()
            .WithMessage(ValidationConsts.EMPTY_PHONE_NUMBER_ERROR)
            .Matches(@"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$");
    }
}