using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс расширенного валидатора комментария задачи.
/// </summary>
public class TaskCommentExtendedValidator : AbstractValidator<TaskCommentExtendedInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public TaskCommentExtendedValidator()
    {
        RuleFor(p => p.ProjectTaskId)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_TASK_ID)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_TASK_ID);
        
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);
            
        RuleFor(p => p.Comment)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.EMPTY_TASK_COMMENT)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.EMPTY_TASK_COMMENT);
            
        RuleFor(p => p.CommentId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.EMPTY_TASK_COMMENT_ID);
    }
}