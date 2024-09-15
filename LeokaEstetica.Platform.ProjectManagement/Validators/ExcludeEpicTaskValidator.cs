using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagement.Validators;

/// <summary>
/// Класс валидатора исключения задач из эпика.
/// </summary>
public class ExcludeEpicTaskValidator : AbstractValidator<ExcludeEpicTaskInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ExcludeEpicTaskValidator()
    {
        RuleFor(p => p.EpicId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_EPIC_ID);

        RuleFor(p => p.ProjectTaskIds)
            .Must(p => p is not null && p.All(x => !string.IsNullOrWhiteSpace(x)))
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_EPIC_ID);
    }
}