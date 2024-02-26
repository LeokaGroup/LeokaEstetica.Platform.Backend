using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора создания статуса задачи.
/// </summary>
public class CreateTaskStatusValidator : AbstractValidator<CreateTaskStatusInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public CreateTaskStatusValidator()
    {
        RuleFor(p => p.StatusName)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.EMPTY_STATUS_NAME)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.EMPTY_STATUS_NAME);

        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);
        
        RuleFor(p => p.AssociationStatusSysName)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.ASSOCIATION_SYS_NAME)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.ASSOCIATION_SYS_NAME);
    }
}