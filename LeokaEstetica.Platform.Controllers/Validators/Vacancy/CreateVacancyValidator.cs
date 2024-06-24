using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;

namespace LeokaEstetica.Platform.Controllers.Validators.Vacancy;

/// <summary>
/// Класс валидатора создания вакансии.
/// </summary>
public class CreateVacancyValidator : AbstractValidator<VacancyInput>
{
    public CreateVacancyValidator()
    {
        RuleFor(p => p.VacancyName)
            .NotEmpty()
            .WithMessage(ValidationConst.Vacancy.EMPTY_VACANCY_NAME)
            .NotNull()
            .WithMessage(ValidationConst.Vacancy.EMPTY_VACANCY_NAME);

        RuleFor(p => p.VacancyName)
            .NotEmpty()
            .WithMessage(ValidationConst.Vacancy.EMPTY_VACANCY_TEXT)
            .NotNull()
            .WithMessage(ValidationConst.Vacancy.EMPTY_VACANCY_TEXT);

        RuleFor(p => p.Payment)
            .NotNull()
            .Matches("^[0-9]+$")
            .WithMessage(ValidationConst.Vacancy.NOT_VALID_PAYMENT);
    }
}