using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.CallCenter.Abstractions.Ticket;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Ticket;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Models.Entities.Ticket;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.CallCenter.Services.Ticket;

/// <summary>
/// Класс реализует методы сервиса тикетов.
/// </summary>
internal sealed class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ILogger<TicketService> _logger;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="ticketRepository">Репозиторий тикетов.</param>
    /// <param name="ticketRepository">Логер.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    public TicketService(ITicketRepository ticketRepository, 
        ILogger<TicketService> logger, 
        IUserRepository userRepository)
    {
        _ticketRepository = ticketRepository;
        _logger = logger;
        _userRepository = userRepository;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает список категорий тикетов.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Категории тикетов.</returns>
    public async Task<IEnumerable<TicketCategoryEntity>> GetTicketCategoriesAsync()
    {
        try
        {
            var result = await _ticketRepository.GetTicketCategoriesAsync();

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод создает тикет.
    /// </summary>
    /// <param name="title">Название категории тикета.</param>
    /// <param name="message">Сообщение тикета.</param>
    /// <param name="account">Аккаунт.</param>
    public async Task CreateTicketAsync(string title, string message, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var createdTicketId = await _ticketRepository.CreateTicketAsync(title, message, userId);

            if (createdTicketId <= 0)
            {
                throw new InvalidOperationException("Ошибка при создании тикета." +
                                                    $"TicketId: {createdTicketId}." +
                                                    $"UserId: {userId}." +
                                                    $"Title: {title}." +
                                                    $"Message: {message}");
            }
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}