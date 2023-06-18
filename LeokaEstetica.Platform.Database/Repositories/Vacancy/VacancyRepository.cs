using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Core.Helpers;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Configs;
using LeokaEstetica.Platform.Models.Entities.Vacancy;
using Microsoft.EntityFrameworkCore;
using IsolationLevel = System.Data.IsolationLevel;

namespace LeokaEstetica.Platform.Database.Repositories.Vacancy;

/// <summary>
/// Класс реализует методы репозитория вакансий. 
/// </summary>
internal sealed class VacancyRepository : IVacancyRepository
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

    #region Публичные методы.

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
    /// <param name="vacancyInput">Входная модель.</param>
    /// <returns>Данные созданной вакансии.</returns>
    public async Task<UserVacancyEntity> CreateVacancyAsync(VacancyInput vacancyInput)
    {
        var transaction = await _pgContext.Database
            .BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            var vacancy = new UserVacancyEntity
            {
                DateCreated = DateTime.Now,
                VacancyName = vacancyInput.VacancyName,
                VacancyText = vacancyInput.VacancyText,
                WorkExperience = vacancyInput.WorkExperience,
                Employment = vacancyInput.Employment,
                Payment = vacancyInput.Payment,
                UserId = vacancyInput.UserId,
                Demands = vacancyInput.Demands,
                Conditions = vacancyInput.Conditions
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
    /// <returns>Данные вакансии.</returns>
    public async Task<UserVacancyEntity> GetVacancyByVacancyIdAsync(long vacancyId)
    {
        var result = await _pgContext.UserVacancies
            .FirstOrDefaultAsync(v => v.VacancyId == vacancyId);

        return result;
    }

    /// <summary>
    /// Метод обновляет вакансию.
    /// </summary>
    /// <param name="vacancyInput">Входная модель.</param>
    /// <returns>Данные вакансии.</returns>
    public async Task<UserVacancyEntity> UpdateVacancyAsync(VacancyInput vacancyInput)
    {
        var vacancyId = vacancyInput.VacancyId;

        var vacancy = await _pgContext.UserVacancies.FirstOrDefaultAsync(v => v.VacancyId == vacancyId
                                                                              && v.UserId == vacancyInput.UserId);

        if (vacancy is null)
        {
            throw new InvalidOperationException($"Не найдено вакансии для обновления. VacancyId был {vacancyId}");
        }

        FillUpdatedVacancy(vacancy, vacancyInput);
        
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
    public async Task<bool> CheckVacancyOwnerAsync(long vacancyId, long userId)
    {
        var result = await _pgContext.UserVacancies.AnyAsync(p => p.VacancyId == vacancyId
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

    /// <summary>
    /// Метод добавляет вакансию в архив.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    public async Task AddVacancyArchiveAsync(long vacancyId, long userId)
    {
        var arvhivedVacancy = new ArchivedVacancyEntity
        {
            VacancyId = vacancyId,
            DateArchived = DateTime.Now,
            UserId = userId
        };

        // Добавляем вакансию в таблицу архивов.
        await _pgContext.ArchivedVacancies.AddAsync(arvhivedVacancy);
        
        // Изменяем статус вакансии на "В архиве".
        await UpdateModerationVacancyStatusAsync(vacancyId, VacancyModerationStatusEnum.ArchivedVacancy);

        await _pgContext.SaveChangesAsync();
    }
    
    /// <summary>
    /// Метод находит название вакансии по ее Id.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Название вакансии.</returns>
    public async Task<string> GetVacancyNameByIdAsync(long vacancyId)
    {
        var result = await _pgContext.UserVacancies
            .Where(v => v.VacancyId == vacancyId)
            .Select(v => v.VacancyName)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод проверяет, находится ли такая вакансия в архиве.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Признак проверки.</returns>
    public async Task<bool> CheckVacancyArchiveAsync(long vacancyId)
    {
        var result = await _pgContext.ArchivedVacancies.AnyAsync(p => p.VacancyId == vacancyId);

        return result;
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод заполняет сущность вакансии для обноления.
    /// </summary>
    /// <param name="vacancy">Вакансия до обновления.</param>
    /// <param name="vacancyInput">Входная модель.</param>
    private void FillUpdatedVacancy(UserVacancyEntity vacancy, VacancyInput vacancyInput)
    {
        vacancy.VacancyName = vacancyInput.VacancyName;
        vacancy.VacancyText = vacancyInput.VacancyText;
        vacancy.WorkExperience = vacancyInput.WorkExperience;
        vacancy.Employment = vacancyInput.Employment;
        vacancy.Payment = vacancyInput.Payment;
        vacancy.Conditions = vacancyInput.Conditions;
        vacancy.Demands = vacancyInput.Demands;
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