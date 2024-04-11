using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора создания задачи проекта.
/// </summary>
public class CreateProjectManagementTaskValidator : AbstractValidator<CreateProjectManagementTaskInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public CreateProjectManagementTaskValidator()
    {
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);
            
        RuleFor(p => p.Name)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_NAME)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_NAME);
    }
}