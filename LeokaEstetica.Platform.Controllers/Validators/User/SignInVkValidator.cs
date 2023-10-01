using FluentValidation;
using LeokaEstetica.Platform.Models.Dto.Input.User;
using LeokaEstetica.Platform.Services.Consts;

namespace LeokaEstetica.Platform.Controllers.Validators.User;

/// <summary>
/// Класс валидатора авторизации пользователя через ВК.
/// </summary>
public class SignInVkValidator : AbstractValidator<UserSignInVkInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public SignInVkValidator()
    {
        RuleFor(p => p.FirstName)
            .NotNull()
            .WithMessage(ValidationConsts.Vk.EMPTY_FIRST_NAME)
            .NotEmpty()
            .WithMessage(ValidationConsts.Vk.EMPTY_FIRST_NAME);
        
        RuleFor(p => p.LastName)
            .NotNull()
            .WithMessage(ValidationConsts.Vk.EMPTY_LAST_NAME)
            .NotEmpty()
            .WithMessage(ValidationConsts.Vk.EMPTY_LAST_NAME);

        RuleFor(p => p.VkUserId)
            .Must(p => p > 0)
            .WithMessage(ValidationConsts.Vk.NOT_VALID_USER_ID);
    }
}