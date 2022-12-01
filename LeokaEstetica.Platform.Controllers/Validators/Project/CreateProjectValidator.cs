using FluentValidation;
using LeokaEstetica.Platform.Controllers.ModelsValidation.Project;
using LeokaEstetica.Platform.Core.Constants;

namespace LeokaEstetica.Platform.Controllers.Validators.Project;

/// <summary>
/// Класс валидатора создания проекта.
/// </summary>
public class CreateProjectValidator : AbstractValidator<CreateProjectValidationModel>
{
    public CreateProjectValidator()
    {
        RuleFor(p => p.ProjectName)
            .NotEmpty()
            .WithMessage(GlobalConfigKeys.ProjectMode.EMPTY_PROJECT_NAME)
            .NotNull()
            .WithMessage(GlobalConfigKeys.ProjectMode.EMPTY_PROJECT_NAME);

        RuleFor(p => p.ProjectDetails)
            .NotEmpty()
            .WithMessage(GlobalConfigKeys.ProjectMode.EMPTY_PROJECT_DETAILS)
            .NotNull()
            .WithMessage(GlobalConfigKeys.ProjectMode.EMPTY_PROJECT_DETAILS);

        RuleFor(p => p.Account)
            .NotNull()
            .WithMessage(GlobalConfigKeys.EMPTY_ACCOUNT)
            .NotEmpty()
            .WithMessage(GlobalConfigKeys.EMPTY_ACCOUNT);
    }
}