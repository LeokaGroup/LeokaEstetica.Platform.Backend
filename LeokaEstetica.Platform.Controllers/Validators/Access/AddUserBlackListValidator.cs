using FluentValidation;
using LeokaEstetica.Platform.Moderation.Models.Dto.Input.Access;
using LeokaEstetica.Platform.Services.Consts;

namespace LeokaEstetica.Platform.Controllers.Validators.Access;

/// <summary>
/// Класс валидатора добавления пользователя в ЧС.
/// </summary>
public class AddUserBlackListValidator : AbstractValidator<AddUserBlackListInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public AddUserBlackListValidator()
    {
        RuleFor(p => p.UserId)
            .Must(p => p > 0)
            .WithMessage(ValidationConsts.ACCESS_NOT_VALID_USER_ID);
    }
}