using LeokaEstetica.Platform.Models.Entities.Ticket;

namespace LeokaEstetica.Platform.CallCenter.Abstractions.Ticket;

/// <summary>
/// Абстракция сервиса тикетов.
/// </summary>
public interface ITicketService
{
    /// <summary>
    /// Метод получает список категорий тикетов.
    /// </summary>
    /// <returns>Категории тикетов.</returns>
    Task<IEnumerable<TicketCategoryEntity>> GetTicketCategoriesAsync();
}