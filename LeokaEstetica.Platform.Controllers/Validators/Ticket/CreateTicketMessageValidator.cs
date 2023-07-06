using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.Ticket;

namespace LeokaEstetica.Platform.Controllers.Validators.Ticket;

/// <summary>
/// Класс валидатора создания сообщения тикета.
/// </summary>
public class CreateTicketMessageValidator : AbstractValidator<CreateMessageInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public CreateTicketMessageValidator()
    {
        RuleFor(p => p.TicketId)
            .NotEmpty()
            .WithMessage(ValidationConst.TicketValidation.NOT_VALID_TICKET_ID)
            .NotNull()
            .WithMessage(ValidationConst.TicketValidation.NOT_VALID_TICKET_ID);

        RuleFor(p => p.Message)
            .NotEmpty()
            .WithMessage(ValidationConst.TicketValidation.EMPTY_MESSAGE)
            .NotNull()
            .WithMessage(ValidationConst.TicketValidation.EMPTY_MESSAGE);
    }
}