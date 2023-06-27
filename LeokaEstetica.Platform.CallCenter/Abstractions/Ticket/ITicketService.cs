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

    /// <summary>
    /// Метод создает тикет.
    /// </summary>
    /// <param name="title">Название категории тикета.</param>
    /// <param name="message">Сообщение тикета.</param>
    /// <param name="account">Аккаунт.</param>
    Task CreateTicketAsync(string title, string message, string account);
}