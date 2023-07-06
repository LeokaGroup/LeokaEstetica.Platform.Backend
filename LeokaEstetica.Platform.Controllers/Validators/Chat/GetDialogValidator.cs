using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Chat.Input;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Controllers.Validators.Chat;

/// <summary>
/// Класс валидатора создания диалога.
/// </summary>
public class GetDialogValidator : AbstractValidator<DialogInput>
{
    public GetDialogValidator()
    {
        RuleFor(p => p.DiscussionType)
            .NotNull()
            .WithMessage(ValidationConst.Chat.NOT_EMPTY_DISCUSSION_TYPE)
            .NotEmpty()
            .WithMessage(ValidationConst.Chat.NOT_EMPTY_DISCUSSION_TYPE)
            .When(p => p.DiscussionType.Equals(DiscussionTypeEnum.Project.ToString()) ||
                       p.DiscussionType.Equals(DiscussionTypeEnum.Vacancy.ToString()))
            .WithMessage(ValidationConst.Chat.NOT_VALID_DISCUSSION_TYPE);
    }
}