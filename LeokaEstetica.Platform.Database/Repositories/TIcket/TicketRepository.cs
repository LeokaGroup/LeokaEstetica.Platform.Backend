using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Ticket;
using LeokaEstetica.Platform.Models.Entities.Ticket;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Database.Repositories.TIcket;

/// <summary>
/// Класс реализует методы репозитория тикетов.
/// </summary>
internal sealed class TicketRepository : ITicketRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public TicketRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает список категорий тикетов.
    /// </summary>
    /// <returns>Категории тикетов.</returns>
    public async Task<IEnumerable<TicketCategoryEntity>> GetTicketCategoriesAsync()
    {
        var result = await _pgContext.TicketCategories.ToListAsync();

        return result;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}