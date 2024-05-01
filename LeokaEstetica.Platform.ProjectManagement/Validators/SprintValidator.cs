using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;

namespace LeokaEstetica.Platform.ProjectManagement.Validators;

/// <summary>
/// Класс валидатора спринта.
/// </summary>
public class SprintValidator : AbstractValidator<(long ProjectSprintId, long ProjectId)>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public SprintValidator()
    {
        RuleFor(p => p.ProjectSprintId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_SPRINT_ID);
        
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);
    }
}