using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора создания метки (тега) проекта.
/// </summary>
public class CreateUserTaskTagValidator : AbstractValidator<ProjectTagInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public CreateUserTaskTagValidator()
    {
        RuleFor(p => p.TagName)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.EMPTY_TAG_NAME)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.EMPTY_TAG_NAME);

        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);
    }
}