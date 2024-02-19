using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора удаления файла задачи.
/// </summary>
public class RemoveTaskFileValidator : AbstractValidator<(long DocumentId, long ProjectId, long ProjectTaskId)>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public RemoveTaskFileValidator()
    {
        RuleFor(p => p.DocumentId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_DOCUMENT_ID);
        
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_TASK_FROM_LINK);
            
        RuleFor(p => p.ProjectTaskId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_TASK_ID);
    }
}