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

    /// <summary>
    /// Метод получает список тикетов для профиля пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список тикетов.</returns>
    Task<IEnumerable<MainInfoTicketEntity>> GetUserProfileTicketsAsync(long userId);

    /// <summary>
    /// Метод получает названия статусов тикетов.
    /// </summary>
    /// <param name="ids">Список Id тикетов, названия которых надо получить.</param>
    /// <returns>Названия статусов тикетов.</returns>
    Task<Dictionary<long, string>> GetTicketStatusNamesAsync(IEnumerable<long> ids);
    
    /// <summary>
    /// Метод получает список тикетов для КЦ.
    /// </summary>
    /// <returns>Список тикетов.</returns>
    Task<IEnumerable<MainInfoTicketEntity>> GetCallCenterTicketsAsync();

    /// <summary>
    /// Метод получает сообщения тикета и связанные данные.
    /// </summary>
    /// <param name="ticketId">Id тикета.</param>
    /// <returns>Сообщения тикета и связанные данные.</returns>
    Task<IEnumerable<TicketMessageEntity>> GetTicketMessagesAsync(long ticketId);
}