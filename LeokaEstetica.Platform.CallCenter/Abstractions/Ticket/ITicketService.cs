using LeokaEstetica.Platform.Models.Dto.Output.Ticket;
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

    /// <summary>
    /// Метод получает список тикетов для профиля пользователя.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список тикетов.</returns>
    Task<IEnumerable<TicketOutput>> GetUserProfileTicketsAsync(string account);

    /// <summary>
    /// Метод получает список тикетов для КЦ.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список тикетов.</returns>
    Task<IEnumerable<TicketOutput>> GetCallCenterTicketsAsync(string account);

    /// <summary>
    /// Метод получает данные тикета.
    /// </summary>
    /// <param name="ticketId">Id тикета.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные тикета.</returns>
    Task<SelectedTicketOutput> GetSelectedTicketAsync(long ticketId, string account);
    
    /// <summary>
    /// Метод создает сообщение тикета.
    /// </summary>
    /// <param name="ticketId">Id тикета.</param>
    /// <param name="message">Сообщение тикета.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список сообщений.</returns>
    Task<CreateTicketMessageOutput> CreateTicketMessageAsync(long ticketId, string message, string account);

    /// <summary>
    /// Метод закрывает тикет (идет проставление статуса тикета "Закрыт").
    /// </summary>
    /// <param name="closeTicketInput">Входная модель.</param>
    Task CloseTicketAsync(long ticketId, string account);

    /// <summary>
    /// Метод создает предложение/пожелание.
    /// </summary>
    /// <param name="contactEmail">Почта пользователя, который оставил пожелание/предложение.</param>
    /// <param name="wisheOfferText">Текст предложение/пожелания.</param>
    Task<WisheOfferOutput> CreateWisheOfferAsync(string contactEmail, string wisheOfferText);
}