using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;

namespace LeokaEstetica.Platform.Controllers.Validators.Project;

/// <summary>
/// Класс валидатора одобрения комментария проекта.
/// </summary>
public class ApproveProjectCommentValidator : AbstractValidator<long>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ApproveProjectCommentValidator()
    {
        RuleFor(p => p)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectCommentValidation.NOT_VALID_COMMENT_ID);
    }
}