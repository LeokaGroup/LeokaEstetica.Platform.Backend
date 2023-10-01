namespace LeokaEstetica.Platform.Models.Dto.Input.Ticket;

/// <summary>
/// Класс входной модели создания тикета.
/// </summary>
public class CreateTicketInput
{
    /// <summary>
    /// Название тикета.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Сообщение тикета.
    /// </summary>
    public string Message { get; set; }
}