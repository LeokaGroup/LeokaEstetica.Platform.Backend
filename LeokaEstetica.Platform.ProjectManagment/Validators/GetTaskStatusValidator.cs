using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.ProjectManagment.ValidationModels;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора статусов задачи проекта.
/// </summary>
public class GetTaskStatusValidator : AbstractValidator<GetTaskStatusValidationModel>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public GetTaskStatusValidator()
    {
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);
    }
}