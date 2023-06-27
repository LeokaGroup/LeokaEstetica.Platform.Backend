using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.Ticket;

namespace LeokaEstetica.Platform.Controllers.Validators.Ticket;

/// <summary>
/// Валидатор создания тикета.
/// </summary>
public class CreateTicketValidator : AbstractValidator<CreateTicketInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public CreateTicketValidator()
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .WithMessage(GlobalConfigKeys.TicketValidation.NOT_VALID_CATEGORY_NAME)
            .NotNull()
            .WithMessage(GlobalConfigKeys.TicketValidation.NOT_VALID_CATEGORY_NAME);

        RuleFor(p => p.Message)
            .NotEmpty()
            .WithMessage(GlobalConfigKeys.TicketValidation.EMPTY_MESSAGE)
            .NotNull()
            .WithMessage(GlobalConfigKeys.TicketValidation.EMPTY_MESSAGE);
    }
}