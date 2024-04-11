using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора доступных к созданию связи задач.
/// </summary>
public class AvailableTaskLinkValidator : AbstractValidator<(long ProjectId, LinkTypeEnum LinkType)>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public AvailableTaskLinkValidator()
    {
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_TASK_FROM_LINK);

        RuleFor(p => p.LinkType)
            .Must(p => new[] { LinkTypeEnum.Child, LinkTypeEnum.Parent, LinkTypeEnum.Depend, LinkTypeEnum.Link }.Contains(p))
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_TASK_LINK_ENUM_VALUE);
    }
}