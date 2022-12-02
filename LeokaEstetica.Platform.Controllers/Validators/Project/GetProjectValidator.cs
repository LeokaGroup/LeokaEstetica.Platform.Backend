using FluentValidation;
using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Controllers.ModelsValidation.Project;
using LeokaEstetica.Platform.Core.Constants;

namespace LeokaEstetica.Platform.Controllers.Validators.Project;

/// <summary>
/// Класс валидатора получения проекта.
/// </summary>
public class GetProjectValidator : AbstractValidator<GetProjectValidationModel>
{
    public GetProjectValidator()
    {
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(p => GlobalConfigKeys.ProjectMode.NOT_VALID_PROJECT_ID + p.ProjectId);
        
        RuleFor(p => p.Mode)
            .Must(p => p != ModeEnum.None)
            .WithMessage(p => GlobalConfigKeys.ProjectMode.EMPTY_MODE + p.Mode);
    }
}