using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Landing;
using LeokaEstetica.Platform.Models.Entities.Landing;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Landing;

/// <summary>
/// Класс реализует методы репозитория лендингов.
/// </summary>
public sealed class LandingRepository : ILandingRepository
{
    private readonly PgContext _pgContext;
    
    public LandingRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод получает данные для блока фона для главного лендоса.
    /// </summary>
    /// <returns>Данные блока.returns>
    public async Task<FonEntity> LandingStartFonAsync()
    {
        var result = await _pgContext.Fons
            .FirstOrDefaultAsync();

        if (result is null)
        {
            throw new ArgumentNullException($"Нет данных фона для главного лендинга!");
        }

        return result;
    }
}