using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Models.Entities.Ticket;

/// <summary>
/// Класс сопоставляется с таблицей файлов тикетов Communications.TicketFiles.
/// </summary>
public class TicketFileEntity
{
    public TicketFileEntity(string url)
    {
        Url = url;
    }
    /// <summary>
    /// PK.
    /// </summary>
    public long FileId { get; set; }

    /// <summary>
    /// Путь к файлу тикета.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Название тикета.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Описание тикета.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Позиция.
    /// </summary>
    public short Position { get; set; }

    /// <summary>
    /// Тип файла тикета.
    /// </summary>
    public TicketFileTypeEnum Type { get; set; }

    /// <summary>
    /// Дата файла.
    /// </summary>
    public DateTime DateCreated { get; set; }
}