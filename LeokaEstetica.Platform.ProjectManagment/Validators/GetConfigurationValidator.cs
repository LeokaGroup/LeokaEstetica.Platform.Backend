using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.ProjectManagment.ValidationModels;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора конфигурации рабочего пространства.
/// </summary>
public class GetConfigurationValidator : AbstractValidator<GetConfigurationValidationModel>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public GetConfigurationValidator()
    {
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);
    }
}