using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Header;
using LeokaEstetica.Platform.Models.Entities.Common;
using LeokaEstetica.Platform.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Database.Repositories.Header;

/// <summary>
/// Класс реализует методы репозитория работы с хидерами.
/// </summary>
internal sealed class HeaderRepository : IHeaderRepository
{
    private readonly PgContext _pgContext;
    private readonly ILogger<HeaderRepository> _logger;

    public HeaderRepository(PgContext pgContext, 
        ILogger<HeaderRepository> logger)
    {
        _pgContext = pgContext;
        _logger = logger;
    }

    /// <summary>
    /// Метод получает список элементов для меню хидера в зависимости от его типа.
    /// </summary>
    /// <param name="headerType">Тип хидера. Например, для лендоса.</param>
    /// <returns>Список элементов для меню хидера.</returns>
    public async Task<IEnumerable<HeaderEntity>> HeaderItemsAsync(HeaderTypeEnum headerType)
    {
        try
        {
            var result = await _pgContext.Header
                .Where(h => h.HeaderType.Equals(headerType))
                .OrderBy(h => h.Position)
                .ToListAsync();

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}