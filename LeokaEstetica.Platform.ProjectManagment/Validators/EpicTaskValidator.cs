using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора задач эпика.
/// </summary>
public class EpicTaskValidator : AbstractValidator<(long ProjectId, long EpicId)>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public EpicTaskValidator()
    {
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);

        RuleFor(p => p.EpicId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_EPIC_ID);
    }
}