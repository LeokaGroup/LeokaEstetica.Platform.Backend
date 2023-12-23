using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора создания задачи проекта.
/// </summary>
public class CreateProjectManagementTaskValidator : AbstractValidator<CreateProjectManagementTaskInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public CreateProjectManagementTaskValidator()
    {
        RuleFor(p => p.Name)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.EMPTY_TASK_NAME)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.EMPTY_TASK_NAME);
        
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);
        
        RuleFor(p => p.TaskStatusId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_STATUS_ID);
            
        RuleFor(p => p.TaskTypeId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_TASK_TYPE_ID);
    }
}