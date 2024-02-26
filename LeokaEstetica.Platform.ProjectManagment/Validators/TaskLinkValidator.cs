using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора связи с задачей.
/// </summary>
public class TaskLinkValidator : AbstractValidator<TaskLinkInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public TaskLinkValidator()
    {
        RuleFor(p => p.TaskFromLink)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_TASK_FROM_LINK)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_TASK_FROM_LINK);

        RuleFor(p => p.TaskToLink)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_TASK_TO_LINK)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_TASK_TO_LINK);

        RuleFor(p => p.LinkType)
            .Must(p => new[] { LinkTypeEnum.Child, LinkTypeEnum.Parent, LinkTypeEnum.Depend, LinkTypeEnum.Link }.Contains(p))
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_TASK_LINK_ENUM_VALUE);
            
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);
    }
}