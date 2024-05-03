using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagement.Validators;

/// <summary>
/// Класс валидатора планирования спринта.
/// </summary>
public class PlaningSprintValidator : AbstractValidator<PlaningSprintInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public PlaningSprintValidator()
    {
        RuleFor(p => p.SprintName)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_SPRINT_NAME)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_SPRINT_NAME);
        
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);
            
        RuleFor(p => p.CreatedBy)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_CREATED_BY);
    }
}