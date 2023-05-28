using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.Project;

namespace LeokaEstetica.Platform.Controllers.Validators.Project;

/// <summary>
/// Класс валидатора создания вакансии проекта.
/// </summary>
public class CreateProjectVacancyValidator : AbstractValidator<CreateProjectVacancyInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public CreateProjectVacancyValidator()
    {
        RuleFor(p => p.VacancyName)
            .NotNull()
            .WithMessage(GlobalConfigKeys.ProjectVacancy.EMPTY_PROJECT_VACANCY_NAME)
            .NotEmpty()
            .WithMessage(GlobalConfigKeys.ProjectVacancy.EMPTY_PROJECT_VACANCY_NAME);

        RuleFor(p => p.VacancyName)
            .NotNull()
            .WithMessage(GlobalConfigKeys.ProjectVacancy.EMPTY_PROJECT_VACANCY_TEXT)
            .NotEmpty()
            .WithMessage(GlobalConfigKeys.ProjectVacancy.EMPTY_PROJECT_VACANCY_TEXT);
    }
}