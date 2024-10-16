using System.Text.RegularExpressions;
using FluentValidation;
using LeokaEstetica.Platform.Models.Dto.Input.User;
using LeokaEstetica.Platform.Services.Consts;

namespace LeokaEstetica.Platform.Controllers.Validators.User;

/// <summary>
/// Класс валидатора проверки аккаунта пользователя для востановения.
/// </summary>
public class CheckRestoreAccountValidator : AbstractValidator<PreRestorePasswordInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public CheckRestoreAccountValidator()
    {
        RuleFor(p => p.Account)
            .NotNull()
            .WithMessage(ValidationConsts.NOT_VALID_EMAIL_ERROR)
            .NotEmpty()
            .WithMessage(ValidationConsts.NOT_VALID_EMAIL_ERROR)
            .Matches(@"^[^@\s]+@(mail\.ru|gmail\.com|inbox\.ru|bk\.ru|list\.ru|yandex\.ru)${2,6}", RegexOptions.IgnoreCase)
            .WithMessage(ValidationConsts.NOT_VALID_EMAIL_ERROR);
    }
}