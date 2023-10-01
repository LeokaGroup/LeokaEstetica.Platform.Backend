namespace LeokaEstetica.Platform.Models.Dto.Output.Ticket;

/// <summary>
/// Класс выходной модели тикетов.
/// </summary>
public class TicketOutput
{
    /// <summary>
    /// Id тикета.
    /// </summary>
    public long TicketId { get; set; }

    /// <summary>
    /// Название тикета.
    /// </summary>
    public string TicketName { get; set; }

    /// <summary>
    /// Название статуса.
    /// </summary>
    public string StatusName { get; set; }
}