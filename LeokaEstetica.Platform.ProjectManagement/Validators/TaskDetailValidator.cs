using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора деталей задачи.
/// </summary>
public class TaskDetailValidator : AbstractValidator<(string ProjectTaskId, long ProjectId)>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public TaskDetailValidator()
    {
        RuleFor(p => p.ProjectTaskId)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_TASK_ID)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_TASK_ID);

        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);
    }
}