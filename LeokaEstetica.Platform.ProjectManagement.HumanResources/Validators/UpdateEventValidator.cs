using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagementHumanResources;

namespace LeokaEstetica.Platform.ProjectManagement.HumanResources.Validators;

/// <summary>
/// Класс валидатора обновления события календаря.
/// </summary>
public class UpdateEventValidator : AbstractValidator<CalendarInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public UpdateEventValidator()
    {
        RuleFor(p => p.EventName)
            .NotNull()
            .WithMessage(ValidationConst.HumanResources.EMPTY_EVENT_NAME)
            .NotEmpty()
            .WithMessage(ValidationConst.HumanResources.EMPTY_EVENT_NAME);

        RuleFor(p => p.EventStartDate)
            .Must(p => p > DateTime.UtcNow)
            .WithMessage(ValidationConst.HumanResources.NOT_VALID_EVENT_START_DATE);

        RuleFor(p => p.EventEndDate)
            .Must(p => p > DateTime.UtcNow)
            .WithMessage(ValidationConst.HumanResources.NOT_VALID_EVENT_END_DATE);
    }
}