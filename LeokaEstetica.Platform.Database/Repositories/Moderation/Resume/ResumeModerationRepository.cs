using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Resume;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Profile;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Moderation.Resume;

/// <summary>
/// Класс реализует методы репозитория модерации анкет пользователей.
/// </summary>
public class ResumeModerationRepository : IResumeModerationRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструтор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public ResumeModerationRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает список анкет для модерации.
    /// </summary>
    /// <returns>Список анкет.</returns>
    public async Task<IEnumerable<ModerationResumeEntity>> ResumesModerationAsync()
    {
        var result = await _pgContext.ModerationResumes
            .Include(r => r.ProfileInfo)
            .Where(p => p.ModerationStatus.StatusId == (int)ResumeModerationStatusEnum.ModerationResume)
            .Select(p => new ModerationResumeEntity
            {
                ModerationId = p.ModerationId,
                ProfileInfoId = p.ProfileInfoId,
                ProfileInfo = new ProfileInfoEntity
                {
                    Aboutme = p.ProfileInfo.Aboutme,
                    FirstName = p.ProfileInfo.FirstName,
                    IsShortFirstName = p.ProfileInfo.IsShortFirstName,
                    Job = p.ProfileInfo.Job,
                    LastName = p.ProfileInfo.LastName,
                    Patronymic = p.ProfileInfo.Patronymic,
                    UserId = p.ProfileInfo.UserId,
                    Telegram = p.ProfileInfo.Telegram,
                    Vkontakte = p.ProfileInfo.Vkontakte,
                    WhatsApp = p.ProfileInfo.WhatsApp
                },
                DateModeration = p.DateModeration
            })
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод отправляет анкету на модерацию. Это происходит через добавление в таблицу модерации анкет.
    /// Если анкета в этой таблице, значит она не прошла еще модерацию. 
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    public async Task AddResumeModerationAsync(long profileInfoId)
    {
        await _pgContext.ModerationResumes.AddAsync(new ModerationResumeEntity
        {
            ProfileInfoId = profileInfoId,
            DateModeration = DateTime.Now,
            ModerationStatusId = (int)ResumeModerationStatusEnum.ModerationResume
        });
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод одобряет анкету на модерации.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    public async Task ApproveResumeAsync(long profileInfoId)
    {
        var profileInfo = await _pgContext.ModerationResumes
            .FirstOrDefaultAsync(p => p.ProfileInfoId == profileInfoId);

        if (profileInfo is null)
        {
            throw new InvalidOperationException(
                $"Не удалось найти анкету для модерации. ProfileInfoId = {profileInfoId}");
        }

        profileInfo.ModerationStatusId = (int)ResumeModerationStatusEnum.ApproveResume;
        await _pgContext.SaveChangesAsync();
    }
    
    /// <summary>
    /// Метод отклоняет анкету на модерации.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    public async Task RejectResumeAsync(long profileInfoId)
    {
        var profileInfo = await _pgContext.ModerationResumes
            .FirstOrDefaultAsync(p => p.ProfileInfoId == profileInfoId);

        if (profileInfo is null)
        {
            throw new InvalidOperationException(
                $"Не удалось найти анкету для модерации. ProfileInfoId = {profileInfoId}");
        }

        profileInfo.ModerationStatusId = (int)ResumeModerationStatusEnum.RejectedResume;
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод обновляет замечания анкеты.
    /// </summary>
    /// <param name="resumeRemarks">Список замечаний для обновления.</param>
    public async Task CreateResumeRemarksAsync(IEnumerable<ResumeRemarkEntity> resumeRemarks)
    {
        await _pgContext.ResumeRemarks.AddRangeAsync(resumeRemarks);
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод получает замечания анкеты, которые ранее были сохранены модератором.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    /// <param name="fields">Список названий полей.</param>
    /// <returns>Список замечаний.</returns>
    public async Task UpdateResumeRemarksAsync(List<ResumeRemarkEntity> resumeRemarks)
    {
        // Проводим все эти манипуляции, чтобы избежать ошибки при обновлении замечаний, которые уже были внесены.
        foreach (var rr in resumeRemarks)
        {
            var local = _pgContext.Set<VacancyRemarkEntity>()
                .Local
                .FirstOrDefault(entry => entry.RemarkId == rr.RemarkId);

            // Если локальная сущность != null.
            if (local != null)
            {
                // Отсоединяем контекст устанавливая флаг Detached.
                _pgContext.Entry(local).State = EntityState.Detached;
            }
            
            // Проставляем обновляемой сущности флаг Modified.
            _pgContext.Entry(rr).State = EntityState.Modified;
        }
        
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод получает замечания анкеты, которые ранее были сохранены модератором.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    /// <param name="fields">Список названий полей.</param>
    /// <returns>Список замечаний.</returns>
    public async Task<List<ResumeRemarkEntity>> GetExistsResumeRemarksAsync(long profileInfoId,
        IEnumerable<string> fields)
    {
        var result = await _pgContext.ResumeRemarks
            .Where(p => p.ProfileInfoId == profileInfoId
                        && fields.Contains(p.FieldName))
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод отправляет замечания вакансии владельцу анкеты.
    /// Отправка замечаний вакансии подразумевает просто изменение статуса замечаниям анкеты.
    /// <param name="profileInfoId">Id анкеты.</param>
    /// </summary>
    public async Task SendResumeRemarksAsync(long profileInfoId)
    {
        var resumeRemarks = await _pgContext.ResumeRemarks
            .Where(pr => pr.ProfileInfoId == profileInfoId)
            .ToListAsync();

        if (resumeRemarks.Any())
        {
            foreach (var rr in resumeRemarks)
            {
                rr.RemarkStatusId = (int)RemarkStatusEnum.AwaitingCorrection;
            }
            
            _pgContext.ResumeRemarks.UpdateRange(resumeRemarks);
            await _pgContext.SaveChangesAsync();
        }
    }

    //// <summary>
    /// Метод проверяет, были ли сохранены замечания анкеты.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    /// <returns>Признак раннего сохранения замечаний.</returns>
    public async Task<bool> CheckResumeRemarksAsync(long profileInfoId)
    {
        var result = await _pgContext.ResumeRemarks
            .Where(pr => pr.ProfileInfoId == profileInfoId)
            .AnyAsync();

        return result;
    }

    /// <summary>
    /// Метод получает замечания анкеты.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    /// <returns>Список замечаний.</returns>
    public async Task<List<ResumeRemarkEntity>> GetResumeRemarksAsync(long profileInfoId)
    {
        var result = await _pgContext.ResumeRemarks
            .Where(pr => pr.ProfileInfoId == profileInfoId
                         && new[]
                         {
                             (int)RemarkStatusEnum.AwaitingCorrection,
                             (int)RemarkStatusEnum.AgainAssigned
                         }.Contains(pr.RemarkStatusId))
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список замечаний анкеты (не отправленные), если они есть.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    /// <returns>Список замечаний анкеты.</returns>
    public async Task<IEnumerable<ResumeRemarkEntity>> GetResumeUnShippedRemarksAsync(long profileInfoId)
    {
        var result = await _pgContext.ResumeRemarks
            .Where(pr => pr.ProfileInfoId == profileInfoId
                         && new[]
                         {
                             (int)RemarkStatusEnum.AwaitingCorrection,
                             (int)RemarkStatusEnum.NotAssigned
                         }.Contains(pr.RemarkStatusId)
                         && pr.ProfileInfoId == profileInfoId)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список замечаний анкеты (не отправленные), если они есть.
    /// Выводим эти данные в таблицу замечаний анкет журнала модерации.
    /// </summary>
    /// <returns>Список замечаний анкеты.</returns>
    public async Task<IEnumerable<ProfileInfoEntity>> GetResumeUnShippedRemarksTableAsync()
    {
        var projectRemarksIds = _pgContext.ResumeRemarks
            .Where(pr => new[]
                {
                    (int)RemarkStatusEnum.NotAssigned,
                    (int)RemarkStatusEnum.Review
                }
                .Contains(pr.RemarkStatusId))
            .Select(pr => pr.ProfileInfoId)
            .AsQueryable();

        var result = await (from pi in _pgContext.ProfilesInfo
                join pr in _pgContext.ResumeRemarks
                    on pi.ProfileInfoId
                    equals pr.ProfileInfoId
                where projectRemarksIds.Contains(pr.ProfileInfoId)
                select new ProfileInfoEntity
                {
                    ProfileInfoId = pi.ProfileInfoId,
                    FirstName = pi.FirstName,
                    LastName = pi.LastName,
                    Patronymic = pi.Patronymic
                })
            .Distinct()
            .ToListAsync();

        return result;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}