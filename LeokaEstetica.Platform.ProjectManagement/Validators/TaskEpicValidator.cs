using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора эпика.
/// </summary>
public class TaskEpicValidator : AbstractValidator<IncludeTaskEpicInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public TaskEpicValidator()
    {
        RuleFor(p => p.ProjectTaskIds)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_TASK_ID)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_TASK_ID)
            .Must(x => x.Any())
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_TASK_ID);

        RuleFor(p => p.EpicId)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_EPIC_ID)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_EPIC_ID);

        RuleFor(p => p.ProjectId)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);
    }
}