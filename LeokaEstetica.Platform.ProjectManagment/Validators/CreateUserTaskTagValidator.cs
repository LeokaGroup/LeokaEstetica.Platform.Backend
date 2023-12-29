using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора создания метки (тега) задач пользователя.
/// </summary>
public class CreateUserTaskTagValidator : AbstractValidator<UserTaskTagInput>
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
    }
}