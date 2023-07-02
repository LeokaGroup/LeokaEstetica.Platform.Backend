using System.Runtime.CompilerServices;
using AutoMapper;
using LeokaEstetica.Platform.CallCenter.Abstractions.Ticket;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Ticket;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Database.Access.Ticket;
using LeokaEstetica.Platform.Models.Dto.Output.Ticket;
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
    private readonly IMapper _mapper;
    private readonly IAccessTicketRepository _accessTicketRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="ticketRepository">Репозиторий тикетов.</param>
    /// <param name="ticketRepository">Логер.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="accessTicketRepository">Репозиторий доступа к тикетам.</param>
    public TicketService(ITicketRepository ticketRepository,
        ILogger<TicketService> logger,
        IUserRepository userRepository,
        IMapper mapper,
        IAccessTicketRepository accessTicketRepository)
    {
        _ticketRepository = ticketRepository;
        _logger = logger;
        _userRepository = userRepository;
        _mapper = mapper;
        _accessTicketRepository = accessTicketRepository;
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
            _logger?.LogError(ex, ex.Message);
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
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список тикетов для профиля пользователя.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список тикетов.</returns>
    public async Task<IEnumerable<TicketOutput>> GetUserProfileTicketsAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var result = new List<TicketOutput>();

            var tickets = await _ticketRepository.GetUserProfileTicketsAsync(userId);

            if (!tickets.Any())
            {
                return result;
            }

            result = _mapper.Map<List<TicketOutput>>(tickets);

            await FillStatusNamesAsync(result);

            return result;
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список тикетов для КЦ.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список тикетов.</returns>
    public async Task<IEnumerable<TicketOutput>> GetCallCenterTicketsAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var roles = await _accessTicketRepository.GetTicketUserRolesAsync(userId);
            var userRoles = roles.ToList();

            // Не даем доступ.
            if (userRoles.Contains(-1))
            {
                // TODO: тут еще бросать уведомлялку как варнинг пользователю об этом. Одним уведомлением сделать.
                throw new InvalidOperationException(
                    $"У пользователя с UserId: {userId} нет прав для доступа к тикетам в КЦ.");
            }

            // Есть ли нужная роль.
            if (!userRoles.Contains(1))
            {
                // TODO: тут еще бросать уведомлялку как варнинг пользователю об этом. Одним уведомлением сделать.
                throw new InvalidOperationException(
                    $"У пользователя с UserId: {userId} нет нужной роли для доступа к тикетам в КЦ.");
            }

            var result = new List<TicketOutput>();

            var tickets = await _ticketRepository.GetCallCenterTicketsAsync();

            if (!tickets.Any())
            {
                return result;
            }

            result = _mapper.Map<List<TicketOutput>>(tickets);

            await FillStatusNamesAsync(result);

            return result;
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает данные тикета.
    /// </summary>
    /// <param name="ticketId">Id тикета.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные тикета.</returns>
    public async Task<SelectedTicketOutput> GetSelectedTicketAsync(long ticketId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var items = await _ticketRepository.GetTicketMessagesAsync(ticketId);
            var result = new SelectedTicketOutput { Messages = new List<TicketMessageOutput>() };

            var ticketMessages = items.ToList();

            if (!ticketMessages.Any())
            {
                // Сообщений нет, но надо получить основные данные тикета.
                var ticket = await _ticketRepository.GetTicketByIdAsync(ticketId);

                if (ticket is null)
                {
                    throw new InvalidOperationException($"Ошибка получения тикета. TicketId: {ticketId}");
                }

                result.TicketName = ticket.TicketName;
                result.TicketId = ticketId;

                await FillStatusNamesAsync(result);

                result.Messages = await CreateMessagesResultAsync(userId, ticket.TicketMessages.ToList());

                return result;
            }

            var first = ticketMessages.FirstOrDefault();
            result.TicketName = first?.MainInfoTicket.TicketName;
            result.TicketId = ticketId;

            await FillStatusNamesAsync(result);
            
            result.Messages = await CreateMessagesResultAsync(userId, ticketMessages);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод создает сообщение тикета.
    /// </summary>
    /// <param name="ticketId">Id тикета.</param>
    /// <param name="message">Сообщение тикета.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список сообщений.</returns>
    public async Task<CreateTicketMessageOutput> CreateTicketMessageAsync(long ticketId, string message, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            var items = await _ticketRepository.CreateTicketMessageAsync(ticketId, message, userId);

            var result = new CreateTicketMessageOutput
            {
                Messages = await CreateMessagesResultAsync(userId, items.ToList())
            };

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

    /// <summary>
    /// Метод проставляет названия статусов тикетов.
    /// </summary>
    /// <param name="tickets">Список тикетов.</param>
    private async Task FillStatusNamesAsync(List<TicketOutput> tickets)
    {
        var ids = tickets.Select(s => s.TicketId);
        var statuses = await _ticketRepository.GetTicketStatusNamesAsync(ids);

        foreach (var t in tickets)
        {
            t.StatusName = statuses.TryGet(t.TicketId);
        }
    }

    /// <summary>
    /// Метод проставляет название статуса тикета.
    /// </summary>
    /// <param name="ticket">Тикет.</param>
    private async Task FillStatusNamesAsync(SelectedTicketOutput ticket)
    {
        var ticketId = ticket.TicketId;
        var statuses = await _ticketRepository.GetTicketStatusNamesAsync(new[] { ticketId });
        ticket.StatusName = statuses.TryGet(ticketId);
    }

    /// <summary>
    /// Метод создает результат сообщений.
    /// </summary>
    /// <param name="ticket">Тикет.</param>
    private async Task<IEnumerable<TicketMessageOutput>> CreateMessagesResultAsync(long userId,
        List<TicketMessageEntity> ticketMessages)
    {
        var result = new List<TicketMessageOutput>();
        
        foreach (var t in ticketMessages)
        {
            var msg = _mapper.Map<TicketMessageOutput>(t);
            
            // Проставляем флаг принадлежности сообщения текущему пользователю.
            msg.IsMyMessage = t.UserId == userId;

            // Форматируем дату сообщения.
            var messageDate = ticketMessages.Find(m => m.MessageId == t.MessageId);
            msg.DateCreated = messageDate?.DateCreated.ToString("g");
            
            result.Add(msg);
        }

        return await Task.FromResult(result);
    }

    #endregion
}