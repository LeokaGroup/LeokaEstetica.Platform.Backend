using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagement.Validators;

/// <summary>
/// Класс валидатора проставления/обновления исполнителя спринта.
/// </summary>
public class InsertOrUpdateSprintExecutorValidator : AbstractValidator<InsertOrUpdateSprintExecutorInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public InsertOrUpdateSprintExecutorValidator()
    {
        RuleFor(p => p.ExecutorId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_TASK_EXECUTOR_ID);
            
        RuleFor(p => p.ProjectSprintId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_SPRINT_ID);
            
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);
    }
}