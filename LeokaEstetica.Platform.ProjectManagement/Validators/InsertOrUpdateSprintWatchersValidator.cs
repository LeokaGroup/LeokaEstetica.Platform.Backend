using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagement.Validators;

/// <summary>
/// Класс валидатора проставления/обновления наблюдателей спринта.
/// </summary>
public class InsertOrUpdateSprintWatchersValidator : AbstractValidator<InsertOrUpdateSprintWatchersInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public InsertOrUpdateSprintWatchersValidator()
    {
        RuleFor(p => p.WatcherIds)
            .Must(p => p is not null && p.Any())
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_TASK_WATCHER_ID);
            
        RuleFor(p => p.ProjectSprintId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_SPRINT_ID);
            
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);
    }
}