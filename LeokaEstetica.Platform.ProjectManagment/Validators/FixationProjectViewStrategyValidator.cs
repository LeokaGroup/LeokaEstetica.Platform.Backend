using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора выбранной стратегии представления.
/// </summary>
public class FixationProjectViewStrategyValidator : AbstractValidator<FixationProjectViewStrategyInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public FixationProjectViewStrategyValidator()
    {
        RuleFor(p => p.StrategySysName)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_STRATEGY_SYS_NAME)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_STRATEGY_SYS_NAME);
            
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_TASK_FROM_LINK);
    }
}