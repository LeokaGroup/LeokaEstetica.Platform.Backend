namespace LeokaEstetica.Platform.Models.Dto.Output.Ticket;

/// <summary>
/// Класс выходной модели выбранного тикета.
/// </summary>
public class SelectedTicketOutput : TicketOutput
{
    /// <summary>
    /// Список сообщений тикета.
    /// </summary>
    public IEnumerable<TicketMessageOutput> Messages { get; set; }

    /// <summary>
    /// Кол-во сообщений.
    /// </summary>
    public int Total => Messages.Count();

    /// <summary>
    /// Признак доступности кнопки отправки сообщения.
    /// </summary>
    public bool IsDisableSendButton { get; set; }

    /// <summary>
    /// Признак доступности кнопки закрытия тикета.
    /// </summary>
    public bool IsDisableCloseTicketButton { get; set; }
}