using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.Resume;
using LeokaEstetica.Platform.Models.Entities.Profile;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Resume;

/// <summary>
/// Класс реализует методы репозитория базы резюме.
/// </summary>
internal sealed class ResumeRepository : IResumeRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public ResumeRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает список резюме.
    /// </summary>
    /// <returns>Список резюме.</returns>
    public async Task<List<ProfileInfoEntity>> GetProfileInfosAsync()
    {
        var result = await (from pi in _pgContext.ProfilesInfo
                join us in _pgContext.UserSubscriptions
                    on pi.UserId
                    equals us.UserId
                join s in _pgContext.Subscriptions
                    on us.SubscriptionId
                    equals s.ObjectId
                join mp in _pgContext.ModerationResumes
                    on pi.ProfileInfoId
                    equals mp.ProfileInfoId
                    into table
                from tbl in table.DefaultIfEmpty()
                where pi.ModerationResumes.All(a => a.ProfileInfoId != pi.ProfileInfoId)
                      && !new[]
                          {
                              (int)VacancyModerationStatusEnum.ModerationVacancy,
                              (int)VacancyModerationStatusEnum.RejectedVacancy
                          }
                          .Contains(tbl.ModerationStatusId)
                orderby s.ObjectId descending
                select new ProfileInfoEntity
                {
                    ProfileInfoId = pi.ProfileInfoId,
                    Aboutme = pi.Aboutme,
                    FirstName = pi.FirstName,
                    IsShortFirstName = pi.IsShortFirstName,
                    Job = pi.Job,
                    LastName = pi.LastName,
                    OtherLink = pi.OtherLink,
                    WhatsApp = pi.WhatsApp,
                    Telegram = pi.Telegram,
                    Vkontakte = pi.Vkontakte,
                    Patronymic = pi.Patronymic,
                    UserId = pi.UserId,
                    WorkExperience = pi.WorkExperience
                })
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
                UserId = pi.UserId,
                ProfileInfoId = pi.ProfileInfoId
            })
            .AsQueryable();

        return await Task.FromResult(result);
    }
    
    /// <summary>
    /// Метод получает заполненные резюме для фильтрации без выгрузки в память.
    /// </summary>
    /// <returns>Резюме без выгрузки в память.</returns>
    public async Task<IOrderedQueryable<ProfileInfoEntity>> GetFilledResumesAsync()
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
                UserId = pi.UserId,
                ProfileInfoId = pi.ProfileInfoId
            }).Where(p =>
                p.FirstName != ""
                || p.LastName != ""
                || p.Job != ""
                || p.Aboutme != "")
            .AsQueryable();

        return await Task.FromResult(result);
    }

    /// <summary>
    /// Метод получает анкету пользователя по ее Id.
    /// </summary>
    /// <param name="resumeId">Id анкеты пользователя.</param>
    /// <returns>Данные анкеты.</returns>
    public async Task<ProfileInfoEntity> GetResumeAsync(long resumeId)
    {
        var result = await _pgContext.ProfilesInfo
            .FirstOrDefaultAsync(p => p.ProfileInfoId == resumeId);

        return result;
    }

    /// <summary>
    /// Метод получает анкеты пользователей по Id анкет.
    /// </summary>
    /// <param name="usersIds">Id пользователей.</param>
    /// <returns>Список анкет.</returns>
    public async Task<IEnumerable<ProfileInfoEntity>> GetResumesAsync(IEnumerable<long> usersIds)
    {
        var result = await _pgContext.ProfilesInfo
            .Where(p => usersIds.Contains(p.UserId))
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод првоеряет владельца анкеты.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Признак является ли пользователь владельцем анкеты.</returns>
    public async Task<bool> CheckResumeOwnerAsync(long profileInfoId, long userId)
    {
        var result = await _pgContext.ProfilesInfo
            .AnyAsync(p => p.ProfileInfoId == profileInfoId
                           && p.UserId == userId);

        return result;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}