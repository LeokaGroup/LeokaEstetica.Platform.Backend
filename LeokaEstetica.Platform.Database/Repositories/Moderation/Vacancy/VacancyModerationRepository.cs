using System.Data;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Vacancy;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Moderation.Vacancy;

/// <summary>
/// Класс реализует методы репозитория модерации вакансий.
/// </summary>
public sealed class VacancyModerationRepository : IVacancyModerationRepository
{
    private readonly PgContext _pgContext;
    
    public VacancyModerationRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод отправляет вакансию на модерацию. Это происходит через добавление в таблицу модерации вакансий.
    /// Если вакансия в этой таблице, значит она не прошла еще модерацию. При прохождении модерации она удаляется из нее.
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
                DateModeration = DateTime.Now
            };
            await _pgContext.ModerationVacancies.AddAsync(vacancy);
            await _pgContext.SaveChangesAsync();
            
            // Проставляем статус модерации проекта "На модерации".
            await _pgContext.ModerationStatuses.AddAsync(new ModerationStatusEntity
            {
                StatusName = VacancyModerationStatusEnum.ModerationVacancy.GetEnumDescription(),
                StatusSysName = VacancyModerationStatusEnum.ModerationVacancy.ToString()
            });
            await _pgContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        
        catch 
        {
            await transaction.RollbackAsync();
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
        var result = await SetVacancyStatus(vacancyId, VacancyModerationStatusEnum.ApproveVacancy);

        if (!result)
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
    /// Метод устанавливает статус вакансии.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="projectModerationStatus">Статус.</param>
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
}