using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора создания перехода.
/// </summary>
public class CreateTaskTransitionValidator : AbstractValidator<CreateTaskTransitionInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public CreateTaskTransitionValidator()
    {
        RuleFor(p => p.TransitionName)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.EMPTY_TRANSITION_NAME)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.EMPTY_TRANSITION_NAME);
    }
}