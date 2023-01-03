using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Resume;
using LeokaEstetica.Platform.Models.Entities.Profile;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Resume;

/// <summary>
/// Класс реализует методы репозитория базы резюме.
/// </summary>
public sealed class ResumeRepository : IResumeRepository
{
    private readonly PgContext _pgContext;
    
    public ResumeRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод получает список резюме.
    /// </summary>
    /// <returns>Список резюме.</returns>
    public async Task<List<ProfileInfoEntity>> GetProfileInfosAsync()
    {
        var result = await _pgContext.ProfilesInfo
            .ToListAsync();

        return result;
    }
}