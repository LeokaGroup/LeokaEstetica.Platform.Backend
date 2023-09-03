using FluentValidation;
using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Controllers.ModelsValidation.Vacancy;
using LeokaEstetica.Platform.Core.Constants;

namespace LeokaEstetica.Platform.Controllers.Validators.Vacancy;

public class VacancyValidator : AbstractValidator<VacancyValidationModel>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public VacancyValidator()
    {
        RuleFor(p => p.VacancyId)
            .Must(p => p > 0)
            .WithMessage(p => ValidationConst.ProjectValidation.NOT_VALID_PROJECT_ID + p.VacancyId);
        
        RuleFor(p => p.Mode)
            .Must(p => p != ModeEnum.None)
            .WithMessage(p => ValidationConst.ProjectValidation.EMPTY_MODE + p.Mode);
    }
}