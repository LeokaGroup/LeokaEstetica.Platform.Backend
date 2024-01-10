using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора изменения статуса задачи.
/// </summary>
public class ChangeTaskStatusValidator : AbstractValidator<ChangeTaskStatusInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ChangeTaskStatusValidator()
    {
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);
        
        RuleFor(p => p.ChangeStatusId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_TASK_STATUS_ID);
            
        RuleFor(p => p.TaskId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_TASK_ID);
    }
}