using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.ProjectManagment.ValidationModels;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора исполнителей задачи проекта.
/// </summary>
public class GetTaskExecutorValidator : AbstractValidator<GetTaskExecutorValidationModel>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public GetTaskExecutorValidator()
    {
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);
    }
}