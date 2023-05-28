using FluentValidation;
using LeokaEstetica.Platform.Models.Dto.Input.User;
using LeokaEstetica.Platform.Services.Consts;

namespace LeokaEstetica.Platform.Controllers.Validators.User;

/// <summary>
/// Класс валидатора авторизации пользователя через Google.
/// </summary>
public class SignInGoogleValidator : AbstractValidator<UserSignInGoogleInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public SignInGoogleValidator()
    {
        RuleFor(p => p.GoogleAuthToken)
            .NotNull()
            .WithMessage(ValidationConsts.Google.EMPTY_GOOGLE_AUTH_CODE)
            .NotEmpty()
            .WithMessage(ValidationConsts.Google.EMPTY_GOOGLE_AUTH_CODE);
    }
}