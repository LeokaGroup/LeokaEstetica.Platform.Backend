using FluentValidation;
using LeokaEstetica.Platform.Models.Dto.Input.User;
using LeokaEstetica.Platform.Services.Consts;

namespace LeokaEstetica.Platform.Controllers.Validators.User;

/// <summary>
/// Класс валидатора восстановления пароля.
/// </summary>
public class RestorePasswordValidator : AbstractValidator<RestorePasswordInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public RestorePasswordValidator()
    {
        RuleFor(p => p)
            .Must(p => !string.IsNullOrWhiteSpace(p.RestorePassword))
            .WithMessage(ValidationConsts.EMPTY_PASSWORD_ERROR);
        
        RuleFor(p => p)
            .NotNull()
            .WithMessage(ValidationConsts.EMPTY_EMAIL_ERROR)
            .NotEmpty()
            .WithMessage(ValidationConsts.EMPTY_EMAIL_ERROR);
    }
}