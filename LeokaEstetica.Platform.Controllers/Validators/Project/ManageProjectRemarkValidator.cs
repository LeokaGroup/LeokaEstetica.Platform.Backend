using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;

namespace LeokaEstetica.Platform.Controllers.Validators.Project;

/// <summary>
/// Класс валидатора управления замечаниями проекта.
/// </summary>
public class ManageProjectRemarkValidator : AbstractValidator<IEnumerable<long>>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ManageProjectRemarkValidator()
    {
        RuleFor(p => p)
            .Must(p => p.Any())
            .WithMessage(ValidationConst.ProjectValidation.EMPTY_REMARK_IDS);
    }
}