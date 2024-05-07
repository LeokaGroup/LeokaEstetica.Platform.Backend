using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagement.Validators;

/// <summary>
/// Класс валидатора обновления описания спринта.
/// </summary>
public class UpdateSprintDetailsValidator : AbstractValidator<UpdateSprintDetailsInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public UpdateSprintDetailsValidator()
    {
        RuleFor(p => p.SprintDetails)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.EMPTY_SPRINT_DETAILS)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.EMPTY_SPRINT_DETAILS);
            
        RuleFor(p => p.ProjectSprintId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_SPRINT_ID);
            
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);
    }
}