using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора разрыва связи между задачами.
/// </summary>
public class RemoveTaskLinkValidator : AbstractValidator<RemoveTaskLinkInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public RemoveTaskLinkValidator()
    {
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_TASK_FROM_LINK);

        RuleFor(p => p.LinkType)
            .Must(p => new[] { LinkTypeEnum.Child, LinkTypeEnum.Parent, LinkTypeEnum.Depend, LinkTypeEnum.Link }
                .Contains(p))
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_TASK_LINK_ENUM_VALUE);
        
        RuleFor(p => p.CurrentTaskId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_TASK_ID);
            
        RuleFor(p => p.RemovedLinkId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_TASK_LINK_REMOVED_TASK_ID);
    }
}