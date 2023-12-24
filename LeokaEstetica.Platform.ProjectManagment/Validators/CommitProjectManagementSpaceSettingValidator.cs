using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.Config;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора фиксации настроек рабочего пространства проекта.
/// </summary>
public class CommitProjectManagementSpaceSettingValidator : AbstractValidator<ConfigSpaceSettingInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public CommitProjectManagementSpaceSettingValidator()
    {
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);
        
        RuleFor(p => p.TemplateId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_TEMPLATE_ID);
            
        RuleFor(p => p.Strategy)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.EMPTY_STRATEGY)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.EMPTY_STRATEGY);
    }
}