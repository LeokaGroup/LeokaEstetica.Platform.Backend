using LeokaEstetica.Platform.Models.Entities.Ticket;

namespace LeokaEstetica.Platform.Database.Abstractions.Ticket;

/// <summary>
/// Абстракция репозитория тикетов.
/// </summary>
public interface ITicketRepository
{
    /// <summary>
    /// Метод получает список категорий тикетов.
    /// </summary>
    /// <returns>Категории тикетов.</returns>
    Task<IEnumerable<TicketCategoryEntity>> GetTicketCategoriesAsync();
}