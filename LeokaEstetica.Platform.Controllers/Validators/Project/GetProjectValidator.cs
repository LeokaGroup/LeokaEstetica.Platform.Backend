using FluentValidation;
using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Controllers.ModelsValidation.Project;
using LeokaEstetica.Platform.Core.Constants;

namespace LeokaEstetica.Platform.Controllers.Validators.Project;

/// <summary>
/// Класс валидатора получения проекта.
/// </summary>
public class ProjectValidator : AbstractValidator<ProjectValidationModel>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ProjectValidator()
    {
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(p => ValidationConst.ProjectValidation.NOT_VALID_PROJECT_ID + p.ProjectId);
        
        RuleFor(p => p.Mode)
            .Must(p => p != ModeEnum.None)
            .WithMessage(p => ValidationConst.ProjectValidation.EMPTY_MODE + p.Mode);
    }
}