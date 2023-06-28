using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Access.Ticket;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Database.Repositories.Access.Ticket;

/// <summary>
/// Класс реализует методы репозитория доступа тикетов.
/// </summary>
internal sealed class AccessTicketRepository : IAccessTicketRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструктор.
    /// <param name="pgContext">Датаконтекст.</param>
    /// </summary>
    public AccessTicketRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает список ролей пользователя для доступа к тикетам.
    /// Вернет -1, если у пользователя вообще нет прав на доступ к тикетам в КЦ.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список ролей пользователя.</returns>
    public async Task<IEnumerable<int>> GetTicketUserRolesAsync(long userId)
    {
        var result = await _pgContext.UserTicketRoles
            .Where(r => r.UserId == userId)
            .Select(r => r.RoleId)
            .ToListAsync();
        
        if (!result.Any())
        {
            return new[] { -1 };
        }

        return result;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}