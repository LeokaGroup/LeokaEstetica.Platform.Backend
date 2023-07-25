namespace LeokaEstetica.Platform.Models.Dto.Input.Ticket;

/// <summary>
/// Класс входной модели создания сообщения тикета.
/// </summary>
public class CreateMessageInput
{
    /// <summary>
    /// Id тикета.
    /// </summary>
    public long TicketId { get; set; }

    /// <summary>
    /// Сообщение.
    /// </summary>
    public string Message { get; set; }
}