using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора изменения описания задачи.
/// </summary>
public class ChangeTaskDetailsValidator : AbstractValidator<ChangeTaskDetailsInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ChangeTaskDetailsValidator()
    {
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);

        RuleFor(p => p.TaskId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_TASK_ID);
    }
}