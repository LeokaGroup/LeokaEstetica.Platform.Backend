using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagement.Validators;

/// <summary>
/// Класс валидатора обновления названия спринта.
/// </summary>
public class UpdateSprintNameValidator : AbstractValidator<UpdateSprintNameInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public UpdateSprintNameValidator()
    {
        RuleFor(p => p.SprintName)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.EMPTY_SPRINT_NAME)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.EMPTY_SPRINT_NAME);
            
        RuleFor(p => p.ProjectSprintId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_SPRINT_ID);
            
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);
    }
}