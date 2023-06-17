using System.Data;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Notification;
using LeokaEstetica.Platform.Models.Entities.Vacancy;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Moderation.Vacancy;

/// <summary>
/// Класс реализует методы репозитория модерации вакансий.
/// </summary>
internal sealed class VacancyModerationRepository : IVacancyModerationRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public VacancyModerationRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод отправляет вакансию на модерацию. Это происходит через добавление в таблицу модерации вакансий.
    /// Если вакансия в этой таблице, значит она не прошла еще модерацию.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    public async Task AddVacancyModerationAsync(long vacancyId)
    {
        var transaction = await _pgContext.Database
            .BeginTransactionAsync(IsolationLevel.ReadCommitted);
        
        try
        {
            // Отправляем вакансию на модерацию.
            var vacancy = new ModerationVacancyEntity
            {
                VacancyId = vacancyId,
                DateModeration = DateTime.Now,
                ModerationStatusId = (int)VacancyModerationStatusEnum.ModerationVacancy
            };
            
            // Если вакансия уже была на модерации, то обновим статус.
            var isModerationExists = await IsModerationExistsVacancyAsync(vacancy.VacancyId);
            
            if (!isModerationExists)
            {
                // Отправляем вакансию на модерацию.
                await SendModerationVacancyAsync(vacancy.VacancyId);
            }
            
            else
            {
                await UpdateModerationVacancyStatusAsync(vacancy.VacancyId,
                    VacancyModerationStatusEnum.ModerationVacancy);
            }

            await _pgContext.SaveChangesAsync();
            
            await transaction.CommitAsync();
        }
        
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Метод получает вакансию для просмотра.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Данные вакансии.</returns>
    public async Task<UserVacancyEntity> GetVacancyModerationByVacancyIdAsync(long vacancyId)
    {
        var result = await _pgContext.UserVacancies
            .FirstOrDefaultAsync(v => v.VacancyId == vacancyId);

        return result;
    }

    /// <summary>
    /// Метод получает список вакансий для модерации.
    /// </summary>
    /// <returns>Список вакансий.</returns>
    public async Task<IEnumerable<ModerationVacancyEntity>> VacanciesModerationAsync()
    {
        var result = await _pgContext.ModerationVacancies
            .Include(up => up.UserVacancy)
            .Where(p => p.ModerationStatus.StatusId == (int)VacancyModerationStatusEnum.ModerationVacancy)
            .Select(p => new ModerationVacancyEntity
            {
                ModerationId = p.ModerationId,
                VacancyId = p.VacancyId,
                UserVacancy = new UserVacancyEntity
                {
                    VacancyName = p.UserVacancy.VacancyName,
                    DateCreated = p.UserVacancy.DateCreated
                },
                DateModeration = p.DateModeration,
                ModerationStatusId = p.ModerationStatusId,
                ModerationStatus = p.ModerationStatus
            })
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод одобряет вакансию на модерации.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Признак подтверждения вакансии.</returns>
    public async Task<bool> ApproveVacancyAsync(long vacancyId)
    {
        var isSuccessSetStatus = await SetVacancyStatus(vacancyId, VacancyModerationStatusEnum.ApproveVacancy);

        if (!isSuccessSetStatus)
        {
            return false;
        }

        var vacancy = await _pgContext.UserVacancies
            .FirstOrDefaultAsync(v => v.VacancyId == vacancyId);

        if (vacancy is null)
        {
            throw new InvalidOperationException($"Не удалось найти вакансию. VacancyId = {vacancyId}");
        }
        
        // Добавляем вакансию в каталог.
        await _pgContext.CatalogVacancies.AddAsync(new CatalogVacancyEntity
        {
            VacancyId = vacancyId
        });
        await _pgContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Метод отклоняет вакансию на модерации.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Признак подтверждения вакансии.</returns>
    public async Task<bool> RejectVacancyAsync(long vacancyId)
    {
        var result = await SetVacancyStatus(vacancyId, VacancyModerationStatusEnum.RejectedVacancy);

        return result;
    }

    /// <summary>
    /// Метод отправляет уведомление в приложении при одобрении вакансии модератором.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя, которому отправим уведомление в приложении.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="projectId">Id проекта.</param>
    public async Task AddNotificationApproveVacancyAsync(long vacancyId, long userId, string vacancyName,
        long projectId)
    {
        await _pgContext.Notifications.AddAsync(new NotificationEntity
        {
            ProjectId = projectId,
            VacancyId = vacancyId,
            UserId = userId,
            NotificationName = NotificationTypeEnum.ApproveModerationVacancy.GetEnumDescription(),
            NotificationSysName = NotificationTypeEnum.ApproveModerationVacancy.ToString(),
            IsNeedAccepted = true,
            Approved = false,
            Rejected = false,
            NotificationText = $"Вакансия \"{vacancyName}\" одобрена модератором",
            Created = DateTime.Now,
            NotificationType = NotificationTypeEnum.ApproveModerationVacancy.ToString(),
            IsShow = true,
            IsOwner = false
        });
        
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод отправляет уведомление в приложении при отклонении вакансии модератором.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя, которому отправим уведомление в приложении.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="projectId">Id проекта.</param>
    public async Task AddNotificationRejectVacancyAsync(long vacancyId, long userId, string vacancyName, 
        long projectId)
    {
        await _pgContext.Notifications.AddAsync(new NotificationEntity
        {
            ProjectId = projectId,
            VacancyId = vacancyId,
            UserId = userId,
            NotificationName = NotificationTypeEnum.RejectModerationVacancy.GetEnumDescription(),
            NotificationSysName = NotificationTypeEnum.RejectModerationVacancy.ToString(),
            IsNeedAccepted = true,
            Approved = false,
            Rejected = false,
            NotificationText = $"Вакансия \"{vacancyName}\" отклонена модератором",
            Created = DateTime.Now,
            NotificationType = NotificationTypeEnum.RejectModerationVacancy.ToString(),
            IsShow = true,
            IsOwner = false
        });
        
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод получает замечания вакансии, которые ранее были сохранены модератором.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="fields">Список названий полей..</param>
    /// <returns>Список замечаний.</returns>
    public async Task<List<VacancyRemarkEntity>> GetExistsVacancyRemarksAsync(long vacancyId,
        IEnumerable<string> fields)
    {
        var result = await _pgContext.VacancyRemarks
            .Where(p => p.VacancyId == vacancyId
                        && fields.Contains(p.FieldName))
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод создает замечания вакансии.
    /// </summary>
    /// <param name="createVacancyRemarkInput">Список замечаний.</param>
    /// <param name="account">Аккаунт.</param>
    public async Task CreateVacancyRemarksAsync(IEnumerable<VacancyRemarkEntity> vacancyRemarks)
    {
        await _pgContext.VacancyRemarks.AddRangeAsync(vacancyRemarks);
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод обновляет замечания вакансии.
    /// </summary>
    /// <param name="vacancyRemarks">Список замечаний для обновления.</param>
    public async Task UpdateVacancyRemarksAsync(List<VacancyRemarkEntity> vacancyRemarks)
    {
        // Проводим все эти манипуляции, чтобы избежать ошибки при обновлении замечаний, которые уже были внесены.
        foreach (var vr in vacancyRemarks)
        {
            var local = _pgContext.Set<VacancyRemarkEntity>()
                .Local
                .FirstOrDefault(entry => entry.RemarkId == vr.RemarkId);

            // Если локальная сущность != null.
            if (local != null)
            {
                // Отсоединяем контекст устанавливая флаг Detached.
                _pgContext.Entry(local).State = EntityState.Detached;
            }
            
            // Проставляем обновляемой сущности флаг Modified.
            _pgContext.Entry(vr).State = EntityState.Modified;
        }
        
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод отправляет замечания вакансии владельцу вакансии.
    /// Отправка замечаний вакансии подразумевает просто изменение статуса замечаниям вакансии.
    /// <param name="vacancyId">Id вакансии.</param>
    /// </summary>
    public async Task SendVacancyRemarksAsync(long vacancyId)
    {
        var vacancyRemarks = await _pgContext.VacancyRemarks
            .Where(pr => pr.VacancyId == vacancyId)
            .ToListAsync();

        if (vacancyRemarks.Any())
        {
            foreach (var pr in vacancyRemarks)
            {
                pr.RemarkStatusId = (int)RemarkStatusEnum.AwaitingCorrection;
            }
            
            _pgContext.VacancyRemarks.UpdateRange(vacancyRemarks);
            await _pgContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Метод проверяет, были ли сохранены замечания вакансии.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Признак раннего сохранения замечаний.</returns>
    public async Task<bool> CheckVacancyRemarksAsync(long vacancyId)
    {
        var result = await _pgContext.VacancyRemarks
            .Where(pr => pr.VacancyId == vacancyId)
            .AnyAsync();

        return result;
    }

    /// <summary>
    /// Метод получает замечания вакансии.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Список замечаний.</returns>
    public async Task<List<VacancyRemarkEntity>> GetVacancyRemarksAsync(long vacancyId)
    {
        var result = await _pgContext.VacancyRemarks
            .Where(pr => pr.VacancyId == vacancyId
                         && new[]
                         {
                             (int)RemarkStatusEnum.AwaitingCorrection,
                             (int)RemarkStatusEnum.AgainAssigned
                         }.Contains(pr.RemarkStatusId))
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список замечаний вакансии (не отправленные), если они есть.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Список замечаний вакансии.</returns>
    public async Task<IEnumerable<VacancyRemarkEntity>> GetVacancyUnShippedRemarksAsync(long vacancyId)
    {
        var result = await _pgContext.VacancyRemarks
            .Where(pr => new[]
                             {
                                 (int)RemarkStatusEnum.NotAssigned,
                                 (int)RemarkStatusEnum.Review
                             }
                             .Contains(pr.RemarkStatusId)
                         && pr.VacancyId == vacancyId)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список замечаний вакансии (не отправленные), если они есть.
    /// Выводим эти данные в таблицу замечаний вакансии журнала модерации.
    /// </summary>
    /// <returns>Список замечаний вакансии.</returns>
    public async Task<IEnumerable<UserVacancyEntity>> GetVacancyUnShippedRemarksTableAsync()
    {
        var projectRemarksIds = _pgContext.VacancyRemarks
            .Where(pr => new[]
                {
                    (int)RemarkStatusEnum.NotAssigned,
                    (int)RemarkStatusEnum.Review
                }
                .Contains(pr.RemarkStatusId))
            .Select(pr => pr.VacancyId)
            .AsQueryable();

        var result = await (from v in _pgContext.UserVacancies
                join vr in _pgContext.VacancyRemarks
                    on v.VacancyId
                    equals vr.VacancyId
                where projectRemarksIds.Contains(vr.VacancyId)
                select new UserVacancyEntity
                {
                    VacancyId = vr.VacancyId,
                    VacancyName = v.VacancyName
                })
            .Distinct()
            .ToListAsync();

        return result;
    }

    
    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод устанавливает статус вакансии.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="vacancyModerationStatus">Статус.</param>
    /// <returns>Признак подвверждения вакансии.</returns>
    private async Task<bool> SetVacancyStatus(long vacancyId, VacancyModerationStatusEnum vacancyModerationStatus)
    {
        var vac = await _pgContext.ModerationVacancies
            .FirstOrDefaultAsync(p => p.VacancyId == vacancyId);

        if (vac is null)
        {
            throw new InvalidOperationException($"Не удалось найти вакансию для модерации. VacancyId = {vacancyId}");
        }

        vac.ModerationStatusId = (int)vacancyModerationStatus;
        await _pgContext.SaveChangesAsync();

        return true;
    }
    
    /// <summary>
    /// Метод проверяет, была ли уже такая вакансия на модерации. 
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Признак модерации.</returns>
    private async Task<bool> IsModerationExistsVacancyAsync(long vacancyId)
    {
        var result = await _pgContext.ModerationVacancies
            .AnyAsync(p => p.VacancyId == vacancyId);

        return result;
    }
    
    /// <summary>
    /// Метод отправляет вакансию на модерацию.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    private async Task SendModerationVacancyAsync(long vacancyId)
    {
        // Добавляем проект в таблицу модерации вакансий.
        await _pgContext.ModerationVacancies.AddAsync(new ModerationVacancyEntity
        {
            DateModeration = DateTime.Now,
            VacancyId = vacancyId,
            ModerationStatusId = (int)VacancyModerationStatusEnum.ModerationVacancy
        });
    }
    
    /// <summary>
    /// Метод обновляет статус вакансии на модерации.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="status">Статус вакансии.</param>
    private async Task UpdateModerationVacancyStatusAsync(long vacancyId, VacancyModerationStatusEnum status)
    {
        var vac = await _pgContext.ModerationVacancies.FirstOrDefaultAsync(p => p.VacancyId == vacancyId);

        if (vac is null)
        {
            throw new InvalidOperationException($"Не найдена вакансия для модерации. VacancyId: {vacancyId}");
        }
        
        vac.ModerationStatusId = (int)status;
    }

    #endregion
}