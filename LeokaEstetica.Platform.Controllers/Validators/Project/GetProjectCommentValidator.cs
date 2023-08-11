using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;

namespace LeokaEstetica.Platform.Controllers.Validators.Project;

/// <summary>
/// Класс валидатора получения комментария проекта.
/// </summary>
public class GetProjectCommentValidator : AbstractValidator<long>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public GetProjectCommentValidator()
    {
        RuleFor(p => p)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectValidation.EMPTY_PROJECT_NAME);
    }
}