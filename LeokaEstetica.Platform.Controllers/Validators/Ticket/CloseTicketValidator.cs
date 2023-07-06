using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;

namespace LeokaEstetica.Platform.Controllers.Validators.Ticket;

/// <summary>
/// Класс валидатора закрытия тикета.
/// </summary>
public class CloseTicketValidator : AbstractValidator<long>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public CloseTicketValidator()
    {
        RuleFor(p => p)
            .Must(p => p > 0)
            .WithMessage(GlobalConfigKeys.TicketValidation.NOT_VALID_TICKET_ID);
    }
}