using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.Project;

namespace LeokaEstetica.Platform.Controllers.Validators.Project;

/// <summary>
/// Класс валидатора обновления проекта.
/// </summary>
public class UpdateProjectValidator : AbstractValidator<UpdateProjectInput>
{
    public UpdateProjectValidator()
    {
        RuleFor(p => p.ProjectName)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectValidation.EMPTY_PROJECT_NAME)
            .NotNull()
            .WithMessage(ValidationConst.ProjectValidation.EMPTY_PROJECT_NAME);

        RuleFor(p => p.ProjectDetails)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectValidation.EMPTY_PROJECT_DETAILS)
            .NotNull()
            .WithMessage(ValidationConst.ProjectValidation.EMPTY_PROJECT_DETAILS);
    }
}