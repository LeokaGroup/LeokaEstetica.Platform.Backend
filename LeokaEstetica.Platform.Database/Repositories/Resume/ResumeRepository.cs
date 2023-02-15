using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Resume;
using LeokaEstetica.Platform.Models.Entities.Profile;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Resume;

/// <summary>
/// Класс реализует методы репозитория базы резюме.
/// </summary>
public class ResumeRepository : IResumeRepository
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
            .Where(r => !string.IsNullOrEmpty(r.FirstName)
                        && !string.IsNullOrEmpty(r.LastName)
                        && !string.IsNullOrEmpty(r.Patronymic)
                        && !string.IsNullOrEmpty(r.Job)
                        && !string.IsNullOrEmpty(r.Aboutme))
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает резюме для фильтрации без выгрузки в память.
    /// </summary>
    /// <returns>Резюме без выгрузки в память.</returns>
    public async Task<IOrderedQueryable<ProfileInfoEntity>> GetFilterResumesAsync()
    {
        var result = (IOrderedQueryable<ProfileInfoEntity>)_pgContext.ProfilesInfo
            .Select(pi => new ProfileInfoEntity
            {
                LastName = pi.LastName,
                FirstName = pi.FirstName,
                Patronymic = pi.Patronymic,
                Job = pi.Job,
                Aboutme = pi.Aboutme,
                IsShortFirstName = pi.IsShortFirstName,
                UserId = pi.UserId
            })
            .AsQueryable();

        return await Task.FromResult(result);
    }

    /// <summary>
    /// Метод получает анкету пользователя.
    /// </summary>
    /// <param name="resumeId">Id анкеты пользователя.</param>
    /// <returns>Данные анкеты.</returns>
    public async Task<ProfileInfoEntity> GetResumeAsync(long resumeId)
    {
        var result = await _pgContext.ProfilesInfo
            .FirstOrDefaultAsync(p => p.ProfileInfoId == resumeId);

        return result;
    }
}