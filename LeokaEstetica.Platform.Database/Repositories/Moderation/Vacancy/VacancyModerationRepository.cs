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
public class VacancyModerationRepository : IVacancyModerationRepository
{
    private readonly PgContext _pgContext;
    
    public VacancyModerationRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

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
}