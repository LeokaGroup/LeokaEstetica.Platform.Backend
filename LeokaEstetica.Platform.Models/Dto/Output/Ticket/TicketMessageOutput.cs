namespace LeokaEstetica.Platform.Models.Dto.Output.Ticket;

/// <summary>
/// Класс выходной модели
/// </summary>
public class TicketMessageOutput : TicketOutput
{
    /// <summary>
    /// PK.
    /// </summary>
    public long MessageId { get; set; }

    /// <summary>
    /// Сообщение.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Дата создания сообщения.
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Признак сообщения текущего пользователя.
    /// </summary>
    public bool IsMyMessage { get; set; }
}