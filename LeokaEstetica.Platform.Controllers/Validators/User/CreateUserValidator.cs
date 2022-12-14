using System.Text.RegularExpressions;
using FluentValidation;
using LeokaEstetica.Platform.Models.Dto.Input.User;
using LeokaEstetica.Platform.Services.Consts;

namespace LeokaEstetica.Platform.Controllers.Validators.User;

/// <summary>
/// Класс валидатора создания пользователя.
/// </summary>
public class CreateUserValidator : AbstractValidator<UserSignUpInput>
{
    public CreateUserValidator()
    {
        RuleFor(p => p.Email)
            .NotNull()
            .WithMessage(ValidationConsts.NOT_VALID_EMAIL_ERROR)
            .NotEmpty()
            .WithMessage(ValidationConsts.NOT_VALID_EMAIL_ERROR)
            .Matches("[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}", RegexOptions.IgnoreCase);

        RuleFor(p => p.Password)
            .NotNull()
            .WithMessage(ValidationConsts.EMPTY_PASSWORD_ERROR)
            .NotEmpty()
            .WithMessage(ValidationConsts.EMPTY_PASSWORD_ERROR);
    }
}