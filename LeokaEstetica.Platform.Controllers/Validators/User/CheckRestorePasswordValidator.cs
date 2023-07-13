using FluentValidation;
using LeokaEstetica.Platform.Services.Consts;

namespace LeokaEstetica.Platform.Controllers.Validators.User;

/// <summary>
/// Класс валидатора проверки восстановления пароля пользователя.
/// </summary>
public class CheckRestorePasswordValidator : AbstractValidator<Guid>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public CheckRestorePasswordValidator()
    {
        RuleFor(p => p)
            .Must(p => p != Guid.Empty)
            .WithMessage(ValidationConsts.NOT_VALID_PUBLIC_ID);
    }
}