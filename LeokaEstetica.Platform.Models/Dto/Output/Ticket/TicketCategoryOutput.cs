namespace LeokaEstetica.Platform.Models.Dto.Output.Ticket;

/// <summary>
/// Класс выходной модели категорий тикетов.
/// </summary>
public class TicketCategoryOutput
{
    /// <summary>
    /// PK.
    /// </summary>
    public short CategoryId { get; set; }

    /// <summary>
    /// Название категории.
    /// </summary>
    public string CategoryName { get; set; }
    
    /// <summary>
    /// Системное название категории.
    /// </summary>
    public string CategorySysName { get; set; }
}