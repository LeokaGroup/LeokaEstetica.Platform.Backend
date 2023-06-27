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
    
    /// <summary>
    /// Метод создает тикет.
    /// </summary>
    /// <param name="title">Название категории тикета.</param>
    /// <param name="message">Сообщение тикета.</param>
    /// <param name="userId">Id пользователя.</param>
    Task<long> CreateTicketAsync(string title, string message, long userId);
}