using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;

namespace LeokaEstetica.Platform.Controllers.Validators.Ticket;

/// <summary>
/// Валидатор получения тикета.
/// </summary>
public class GetTicketValidator : AbstractValidator<long>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public GetTicketValidator()
    {
        RuleFor(p => p)
            .Must(p => p > 0)
            .WithMessage(GlobalConfigKeys.TicketValidation.NOT_VALID_TICKET_ID);
    }
}