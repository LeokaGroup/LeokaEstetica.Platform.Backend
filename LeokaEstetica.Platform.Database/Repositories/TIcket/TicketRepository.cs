using System.Data;
using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.Ticket;
using LeokaEstetica.Platform.Models.Entities.Ticket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Database.Repositories.TIcket;

/// <summary>
/// Класс реализует методы репозитория тикетов.
/// </summary>
internal sealed class TicketRepository : ITicketRepository
{
    private readonly PgContext _pgContext;
    private readonly ILogger<TicketRepository> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    /// <param name="logger">Логер.</param>
    public TicketRepository(PgContext pgContext, 
        ILogger<TicketRepository> logger)
    {
        _pgContext = pgContext;
        _logger = logger;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает список категорий тикетов.
    /// </summary>
    /// <returns>Категории тикетов.</returns>
    public async Task<IEnumerable<TicketCategoryEntity>> GetTicketCategoriesAsync()
    {
        var result = await _pgContext.TicketCategories
            .OrderBy(o => o.Position)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод создает тикет.
    /// </summary>
    /// <param name="title">Название категории тикета.</param>
    /// <param name="message">Сообщение тикета.</param>
    /// <param name="userId">Id пользователя.</param>
    public async Task<long> CreateTicketAsync(string title, string message, long userId)
    {
        var transaction = await _pgContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        
        try
        {
            // Добавляем основную информацию о тикете, тем самым создаем новый тикет.
            var ticket = new MainInfoTicketEntity
            {
                DateCreated = DateTime.UtcNow,
                TicketName = title,
                TicketStatusId = (short)TicketStatusEnum.Opened,
                TicketFileId = null // TODO: Позже доработаем это.
            };
            
            await _pgContext.MainInfoTickets.AddAsync(ticket);
            
            // Потому что нам нужен TicketId. Не страшно, ведь фактического сохранения тут не будет.
            await _pgContext.SaveChangesAsync();

            if (ticket.TicketId <= 0)
            {
                return 0;
            }
            
            // Находим пользователей, у которых есть роль администратора тикетов, чтобы добавить их в участники тикета.
            var adminIds = await GetUserAdminRulesIdsAsync();

            var ticketMembers = new List<TicketMemberEntity>();
            var ticketId = ticket.TicketId;
            
            // Добавляем админов в участники тикета.
            foreach (var id in adminIds)
            {
                // Добавляем админа в тикет.
                ticketMembers.Add(new TicketMemberEntity
                {
                    Joined = DateTime.UtcNow,
                    UserId = id,
                    TicketId = ticketId
                });
            }
            
            // Добавляем пользователя в тикет.
            ticketMembers.Add(new TicketMemberEntity
            {
                Joined = DateTime.UtcNow,
                UserId = userId,
                TicketId = ticketId
            });
            
            await _pgContext.TicketMembers.AddRangeAsync(ticketMembers);
            
            // Создаем сообщение тикета.
            await _pgContext.TicketMessages.AddAsync(new TicketMessageEntity
            {
                DateCreated = DateTime.UtcNow,
                IsMyMessage = true,
                Message = message,
                UserId = userId,
                TicketId = ticketId
            });
            
            await _pgContext.SaveChangesAsync();
            
            await transaction.CommitAsync();

            return ticketId;
        }
        
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список тикетов для профиля пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список тикетов.</returns>
    public async Task<IEnumerable<MainInfoTicketEntity>> GetUserProfileTicketsAsync(long userId)
    {
        var result = await (from it in _pgContext.MainInfoTickets
                join tm in _pgContext.TicketMembers
                    on it.TicketId
                    equals tm.TicketId
                where tm.UserId == userId
                select new MainInfoTicketEntity
                {
                    TicketStatusId = it.TicketStatusId,
                    TicketName = it.TicketName,
                    TicketId = it.TicketId
                })
            .Distinct()
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает названия статусов тикетов.
    /// </summary>
    /// <param name="ids">Список Id тикетов, названия которых надо получить.</param>
    /// <returns>Названия статусов тикетов.</returns>
    public async Task<Dictionary<long, string>> GetTicketStatusNamesAsync(IEnumerable<long> ids)
    {
        var result = await (from it in _pgContext.MainInfoTickets
                join ts in _pgContext.TicketStatuses
                    on it.TicketStatusId
                    equals ts.StatusId
                where ids.Contains(it.TicketId)
                select new
                {
                    ts.StatusName,
                    it.TicketId
                })
            .ToDictionaryAsync(k => k.TicketId, v => v.StatusName);

        return result;
    }

    /// <summary>
    /// Метод получает список тикетов для КЦ.
    /// </summary>
    /// <returns>Список тикетов.</returns>
    public async Task<IEnumerable<MainInfoTicketEntity>> GetCallCenterTicketsAsync()
    {
        var result = await (from it in _pgContext.MainInfoTickets
                join tm in _pgContext.TicketMembers
                    on it.TicketId
                    equals tm.TicketId
                select new MainInfoTicketEntity
                {
                    TicketStatusId = it.TicketStatusId,
                    TicketName = it.TicketName,
                    TicketId = it.TicketId
                })
            .Distinct()
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает сообщения тикета и связанные данные.
    /// </summary>
    /// <param name="ticketId">Id тикета.</param>
    /// <returns>Сообщения тикета и связанные данные.</returns>
    public async Task<IEnumerable<TicketMessageEntity>> GetTicketMessagesAsync(long ticketId)
    {
        var result = await _pgContext.TicketMessages
            .Include(t => t.MainInfoTicket)
            .Include(u => u.User)
            .Where(t => t.TicketId == ticketId)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает тикет по его Id.
    /// </summary>
    /// <param name="ticketId">Id тикета.</param>
    /// <returns>Данные тикета.</returns>
    public async Task<MainInfoTicketEntity> GetTicketByIdAsync(long ticketId)
    {
        var result = await _pgContext.MainInfoTickets.FirstOrDefaultAsync(t => t.TicketId == ticketId);

        return result;
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод находит Id пользователей, у которых есть роль админа тикетов.
    /// </summary>
    /// <returns>Список Id таких пользователей.</returns>
    private async Task<IEnumerable<long>> GetUserAdminRulesIdsAsync()
    {
        // Получаем Id роли админа.
        var adminRuleId = await _pgContext.TicketRoles
            .Where(r => r.RoleSysName.Equals("Admin"))
            .Select(r => r.RoleId)
            .FirstOrDefaultAsync();
        
        // Получаем пользователей админов.
        var result = await _pgContext.UserTicketRoles
            .Where(u => u.RoleId == adminRuleId)
            .Select(u => u.UserId)
            .ToListAsync();

        return result;
    }

    #endregion
}