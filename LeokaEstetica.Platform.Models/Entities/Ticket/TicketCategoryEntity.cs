namespace LeokaEstetica.Platform.Models.Entities.Ticket;

/// <summary>
/// Класс сопоставляется с таблицей категорий тикетов Communications.TicketCategories.
/// </summary>
public class TicketCategoryEntity
{
    public TicketCategoryEntity(string categoryName, string categorySysName)
    {
        CategoryName = categoryName;
        CategorySysName = categorySysName;
    }
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

    /// <summary>
    /// Позиция.
    /// </summary>
    public short Position { get; set; }
}