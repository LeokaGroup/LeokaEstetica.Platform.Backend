namespace LeokaEstetica.Platform.Models.Dto.Output.Ticket;

/// <summary>
/// Класс выходной модели сообщений тикета.
/// </summary>
public class CreateTicketMessageOutput : SelectedTicketOutput
{
    /// <summary>
    /// Признак успеха.
    /// </summary>
    public bool IsSuccess { get; set; }
}