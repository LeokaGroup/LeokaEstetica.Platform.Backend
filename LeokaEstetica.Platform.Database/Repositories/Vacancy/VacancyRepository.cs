using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Core.Helpers;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Configs;
using LeokaEstetica.Platform.Models.Entities.Vacancy;
using Microsoft.EntityFrameworkCore;
using IsolationLevel = System.Data.IsolationLevel;

namespace LeokaEstetica.Platform.Database.Repositories.Vacancy;

/// <summary>
/// Класс реализует методы репозитория вакансий. 
/// </summary>
public class VacancyRepository : IVacancyRepository
{
    private readonly PgContext _pgContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public VacancyRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод получает список меню вакансий.
    /// </summary>
    /// <returns>Список меню.</returns>
    public async Task<VacancyMenuItemEntity> VacanciesMenuItemsAsync()
    {
        var result = await _pgContext.VacancyMenuItems
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод создает вакансию.
    /// </summary>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyText">Описание вакансии.</param>
    /// <param name="workExperience">Опыт работы.</param>
    /// <param name="employment">Занятость у вакансии.</param>
    /// <param name="payment">Оплата у вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Данные созданной вакансии.</returns>
    public async Task<UserVacancyEntity> CreateVacancyAsync(string vacancyName, string vacancyText,
        string workExperience, string employment, string payment, long userId)
    {
        var transaction = await _pgContext.Database
            .BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            var vacancy = new UserVacancyEntity
            {
                DateCreated = DateTime.Now,
                VacancyName = vacancyName,
                VacancyText = vacancyText,
                WorkExperience = workExperience,
                Employment = employment,
                Payment = payment,
                UserId = userId
            };
            await _pgContext.UserVacancies.AddAsync(vacancy);
            
            await _pgContext.SaveChangesAsync(); // Сохраняем тут, так как нам нужен VacancyId.

            // Добавляем вакансию в таблицу статусов вакансий. Проставляем новой вакансии статус "На модерации". 
            await AddVacancyStatusAsync(vacancy.VacancyId, VacancyStatusNameEnum.Moderation.GetEnumDescription(),
                VacancyStatusNameEnum.Moderation.ToString());
            
            await _pgContext.SaveChangesAsync();
            
            await transaction.CommitAsync();

            return vacancy;
        }

        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// TODO: userId возможно нужкн будет использовать, если будет монетизация в каталоге вакансий. Если доступ будет только у тех пользователей, которые приобрели подписку.
    /// Метод получает список вакансий для каталога.
    /// </summary>
    /// <returns>Список вакансий.</returns>
    public async Task<List<CatalogVacancyOutput>> CatalogVacanciesAsync()
    {
        var result = await (from cv in _pgContext.CatalogVacancies
                join mv in _pgContext.ModerationVacancies
                    on cv.VacancyId
                    equals mv.VacancyId
                    into table
                from tbl in table.DefaultIfEmpty()
                where !new[]
                    {
                        (int)VacancyModerationStatusEnum.ModerationVacancy,
                        (int)VacancyModerationStatusEnum.RejectedVacancy
                    }
                    .Contains(tbl.ModerationStatusId)
                    where cv.Vacancy.ArchivedVacancies.All(a => a.VacancyId!= cv.Vacancy.VacancyId)
                select new CatalogVacancyOutput
                {
                    VacancyName = cv.Vacancy.VacancyName,
                    DateCreated = cv.Vacancy.DateCreated,
                    Employment = cv.Vacancy.Employment,
                    Payment = cv.Vacancy.Payment,
                    VacancyId = cv.Vacancy.VacancyId,
                    VacancyText = cv.Vacancy.VacancyText,
                    WorkExperience = cv.Vacancy.WorkExperience,
                    UserId = cv.Vacancy.UserId
                })
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод добавляет статус вакансии.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="statusName">Название статуса.</param>
    /// <param name="statusSysName">Системное название статуса.</param>
    public async Task AddVacancyStatusAsync(long vacancyId, string statusName, string statusSysName)
    {
        var vacancyStatus = new VacancyStatusEntity
        {
            VacancyId = vacancyId,
            VacancyStatusName = statusName,
            VacancyStatusSysName = statusSysName
        };
        await _pgContext.VacancyStatuses.AddAsync(vacancyStatus);
    }

    /// <summary>
    /// Метод получает названия полей для таблицы вакансий проектов пользователя.
    /// Все названия столбцов этой таблицы одинаковые у всех пользователей.
    /// </summary>
    /// <returns>Список названий полей таблицы.</returns>
    public async Task<IEnumerable<ProjectVacancyColumnNameEntity>> ProjectUserVacanciesColumnsNamesAsync()
    {
        var result = await _pgContext.ProjectVacancyColumnsNames
            .OrderBy(o => o.Position)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает вакансию по ее Id.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Данные вакансии.</returns>
    public async Task<UserVacancyEntity> GetVacancyByVacancyIdAsync(long vacancyId, long userId)
    {
        var result = await _pgContext.UserVacancies
            .FirstOrDefaultAsync(v => v.VacancyId == vacancyId);

        return result;
    }

    /// <summary>
    /// Метод обновляет вакансию.
    /// </summary>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyText">Описание вакансии.</param>
    /// <param name="workExperience">Опыт работы.</param>
    /// <param name="employment">Занятость у вакансии.</param>
    /// <param name="payment">Оплата у вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Данные созданной вакансии.</returns>
    public async Task<UserVacancyEntity> UpdateVacancyAsync(string vacancyName, string vacancyText,
        string workExperience, string employment, string payment, long userId, long vacancyId)
    {
        var vacancy = await _pgContext.UserVacancies
            .FirstOrDefaultAsync(v => v.VacancyId == vacancyId
                                      && v.UserId == userId);

        if (vacancy is null)
        {
            throw new InvalidOperationException($"Не найдено вакансии для обновления. VacancyId был {vacancyId}");
        }

        vacancy.VacancyName = vacancyName;
        vacancy.VacancyText = vacancyText;
        vacancy.WorkExperience = workExperience;
        vacancy.Employment = employment;
        vacancy.Payment = payment;
        
        await _pgContext.SaveChangesAsync(); // Сохраняем тут, так как нам нужен VacancyId.

        // Добавляем вакансию в таблицу статусов вакансий. Проставляем новой вакансии статус "На модерации". 
        await AddVacancyStatusAsync(vacancy.VacancyId, VacancyStatusNameEnum.Moderation.GetEnumDescription(),
            VacancyStatusNameEnum.Moderation.ToString());
        
        await _pgContext.SaveChangesAsync();

        return vacancy;
    }
    
    /// <summary>
    /// Метод находит Id владельца проекта.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Id владельца вакансии.</returns>
    public async Task<long> GetVacancyOwnerIdAsync(long vacancyId)
    {
        var result = await _pgContext.UserVacancies
            .Where(v => v.VacancyId == vacancyId)
            .Select(v => v.UserId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает название вакансии по ее Id.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Название вакансии.</returns>
    public async Task<string> GetVacancyNameByVacancyIdAsync(long vacancyId)
    {
        var result = await _pgContext.UserVacancies
            .Where(v => v.VacancyId == vacancyId)
            .Select(v => v.VacancyName)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список вакансий для дальнейшей фильтрации.
    /// </summary>
    /// <returns>Список вакансий без выгрузки в память, так как этот список будем еще фильтровать.</returns>
    public async Task<IOrderedQueryable<CatalogVacancyOutput>> GetFiltersVacanciesAsync()
    {
        var result = (IOrderedQueryable<CatalogVacancyOutput>)_pgContext.CatalogVacancies
            .Include(uv => uv.Vacancy)
            .Select(uv => new CatalogVacancyOutput
            {
                VacancyName = uv.Vacancy.VacancyName,
                DateCreated = uv.Vacancy.DateCreated,
                Employment = uv.Vacancy.Employment,
                Payment = uv.Vacancy.Payment,
                VacancyId = uv.Vacancy.VacancyId,
                VacancyText = uv.Vacancy.VacancyText,
                WorkExperience = uv.Vacancy.WorkExperience
            })
            .AsQueryable();

        return await Task.FromResult(result);
    }

    /// <summary>
    /// Метод удаляет вакансию.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Признак удаления и название вакансии.</returns>
    public async Task<(bool Success, string VacancyName)> DeleteVacancyAsync(long vacancyId, long userId)
    {
        var tran = await _pgContext.Database
            .BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            var catalogVacancy = await _pgContext.CatalogVacancies
                .FirstOrDefaultAsync(v => v.VacancyId == vacancyId 
                                          && v.Vacancy.UserId == userId);

            // Удаляем вакансию из каталога.
            if (catalogVacancy is not null)
            {
                _pgContext.CatalogVacancies.Remove(catalogVacancy);   
            }
            
            var vacancyStatus = await _pgContext.VacancyStatuses
                .FirstOrDefaultAsync(v => v.VacancyId == vacancyId);
            
            // Удаляем вакансию из статусов.
            if (vacancyStatus is not null)
            {
                _pgContext.VacancyStatuses.Remove(vacancyStatus);
            }
            
            var moderationVacancy = await _pgContext.ModerationVacancies
                .FirstOrDefaultAsync(v => v.VacancyId == vacancyId);
            
            // Удаляем вакансию из модерации.
            if (moderationVacancy is not null)
            {
                _pgContext.ModerationVacancies.Remove(moderationVacancy);
            }
            
            var userVacancy = await _pgContext.UserVacancies
                .FirstOrDefaultAsync(v => v.VacancyId == vacancyId 
                                          && v.UserId == userId);

            var result = (Success: false , VacancyName: string.Empty);

            // Удаляем вакансию пользователя.
            if (userVacancy is not null)
            {
                // Записываем название вакансии, которая будет удалена.
                result.VacancyName = userVacancy.VacancyName;
                
                _pgContext.UserVacancies.Remove(userVacancy);
            }

            await _pgContext.SaveChangesAsync();
            await tran.CommitAsync();

            result.Success = true;

            return result;
        }
        
        catch
        {
            await tran.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Метод првоеряет владельца вакансии.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Признак является ли пользователь владельцем вакансии.</returns>
    public async Task<bool> CheckProjectOwnerAsync(long vacancyId, long userId)
    {
        var result = await _pgContext.UserVacancies
            .AnyAsync(p => p.VacancyId == vacancyId
                           && p.UserId == userId);

        return result;
    }

    /// <summary>
    /// Метод получает список вакансий пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список вакансий.</returns>
    public async Task<IEnumerable<UserVacancyEntity>> GetUserVacanciesAsync(long userId)
    {
        var result = await _pgContext.UserVacancies
            .Where(v => v.UserId == userId)
            .OrderBy(v => v.VacancyId)
            .ToListAsync();

        return result;
    }
}