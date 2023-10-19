using FluentValidation;
using LeokaEstetica.Platform.Services.Consts;

namespace LeokaEstetica.Platform.Controllers.Validators.User;

/// <summary>
/// Класс валидатора проверки восстановления пароля пользователя.
/// </summary>
public class CheckRestorePasswordValidator : AbstractValidator<(string ConfirmCode, string Email)>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public CheckRestorePasswordValidator()
    {
        RuleFor(p => p)
            .Must(p => !string.IsNullOrWhiteSpace(p.ConfirmCode))
            .WithMessage(ValidationConsts.NOT_VALID_PUBLIC_ID);
            
        RuleFor(p => p)
            .Must(p => !string.IsNullOrWhiteSpace(string.Format(ValidationConsts.NOT_VALID_EMAIL, p.Email)))
            .WithMessage(ValidationConsts.NOT_VALID_EMAIL);
    }
}