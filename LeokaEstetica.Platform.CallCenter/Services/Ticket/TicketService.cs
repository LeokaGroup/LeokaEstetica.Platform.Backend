using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.CallCenter.Abstractions.Ticket;
using LeokaEstetica.Platform.Database.Abstractions.Ticket;
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

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="ticketRepository">Репозиторий тикетов.</param>
    /// <param name="ticketRepository">Логер.</param>
    public TicketService(ITicketRepository ticketRepository, 
        ILogger<TicketService> logger)
    {
        _ticketRepository = ticketRepository;
        _logger = logger;
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

    #endregion

    #region Приватные методы.

    

    #endregion
}