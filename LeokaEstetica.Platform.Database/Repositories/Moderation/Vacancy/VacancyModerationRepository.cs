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
                DateModeration = p.DateModeration
            })
            .ToListAsync();

        return result;
    }
}