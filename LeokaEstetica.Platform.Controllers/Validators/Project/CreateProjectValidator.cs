using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.Project;

namespace LeokaEstetica.Platform.Controllers.Validators.Project;

/// <summary>
/// Класс валидатора создания проекта.
/// </summary>
public class CreateProjectValidator : AbstractValidator<CreateProjectInput>
{
    public CreateProjectValidator()
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