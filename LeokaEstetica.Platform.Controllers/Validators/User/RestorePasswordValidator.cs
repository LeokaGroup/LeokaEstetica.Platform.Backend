using FluentValidation;
using LeokaEstetica.Platform.Services.Consts;

namespace LeokaEstetica.Platform.Controllers.Validators.User;

/// <summary>
/// Класс валидатора восстановления пароля.
/// </summary>
public class RestorePasswordValidator : AbstractValidator<string>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public RestorePasswordValidator()
    {
        RuleFor(p => p)
            .Must(p => !string.IsNullOrEmpty(p))
            .WithMessage(ValidationConsts.EMPTY_PASSWORD_ERROR);
    }
}