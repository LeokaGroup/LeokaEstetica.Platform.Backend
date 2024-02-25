using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора привязки/отвязки тега к задаче/от задачи.
/// </summary>
public class ProjectTaskTagValidator : AbstractValidator<ProjectTaskTagInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ProjectTaskTagValidator()
    {
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);

        RuleFor(p => p.ProjectTaskId)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_TASK_ID)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_TASK_ID);
            
        RuleFor(p => p.TagId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_TASK_TAG_ID);
    }
}