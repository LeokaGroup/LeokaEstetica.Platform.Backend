using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Core.Helpers;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Configs;
using LeokaEstetica.Platform.Models.Entities.Vacancy;
using LeokaEstetica.Platform.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Enum = System.Enum;
using IsolationLevel = System.Data.IsolationLevel;

namespace LeokaEstetica.Platform.Database.Repositories.Vacancy;

/// <summary>
/// Класс реализует методы репозитория вакансий. 
/// </summary>
internal sealed class VacancyRepository : BaseRepository, IVacancyRepository
{
    private readonly PgContext _pgContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public VacancyRepository(PgContext pgContext,
        IConnectionProvider connectionProvider) : base(connectionProvider)
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

    /// <inheritdoc />
    public async Task<UserVacancyEntity> CreateVacancyAsync(VacancyInput vacancyInput, long userId)
    {
        var transaction = await _pgContext.Database
            .BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            var vacancy = new UserVacancyEntity
            {
                DateCreated = DateTime.UtcNow,
                VacancyName = vacancyInput.VacancyName,
                VacancyText = vacancyInput.VacancyText,
                WorkExperience = vacancyInput.WorkExperience,
                Employment = vacancyInput.Employment,
                Payment = vacancyInput.Payment,
                UserId = userId,
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
                join v in _pgContext.UserVacancies
                    on tbl.UserVacancy.VacancyId
                    equals v.VacancyId
                join us in _pgContext.UserSubscriptions
                    on tbl.UserVacancy.UserId
                    equals us.UserId
                join s in _pgContext.Subscriptions
                    on us.SubscriptionId
                    equals s.ObjectId
                where v.ArchivedVacancies.All(a => a.VacancyId != v.VacancyId)
                      && !new[]
                          {
                              (int)VacancyModerationStatusEnum.ModerationVacancy,
                              (int)VacancyModerationStatusEnum.RejectedVacancy
                          }
                          .Contains(tbl.ModerationStatusId)
                orderby cv.Vacancy.DateCreated descending, s.ObjectId descending
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
            .OrderByDescending(o => o.VacancyId)
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
            DateArchived = DateTime.UtcNow,
            UserId = userId
        };

        // Добавляем вакансию в таблицу архивов.
        await _pgContext.ArchivedVacancies.AddAsync(arvhivedVacancy);
        
        var vac = await _pgContext.ModerationVacancies.FirstOrDefaultAsync(p => p.VacancyId == vacancyId);

        // Изменяем статус вакансии на "В архиве", если она есть на модерации.
        if (vac is not null)
        {
            vac.ModerationStatusId = (int)VacancyModerationStatusEnum.ArchivedVacancy;
        }

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

    /// <summary>
    /// Метод удаляет из архива вакансию.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    public async Task<bool> DeleteVacancyArchiveAsync(long vacancyId, long userId)
    {
        var transaction = await _pgContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            var va = await _pgContext.ArchivedVacancies.FirstOrDefaultAsync(p => p.VacancyId == vacancyId
                && p.UserId == userId);
        
            if (va is null)
            {
                return false;
            }

            _pgContext.ArchivedVacancies.Remove(va);
            
            await _pgContext.SaveChangesAsync();

            await transaction.CommitAsync();
        
            return true;
        }
        
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Метод получает список вакансий пользователя из архива.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список архивированных вакансий.</returns>
    public async Task<IEnumerable<ArchivedVacancyEntity>> GetUserVacanciesArchiveAsync(long userId)
    {
        var result = await _pgContext.ArchivedVacancies
            .Include(a => a.UserVacancy)
            .Where(a => a.UserId == userId)
            .ToListAsync();

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CatalogVacancyOutput>> GetCatalogVacanciesAsync(VacancyCatalogInput VacancyCatalogInput)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        // Если передали любой из фильтров сортировки.
        var isNeedOrder = false;

        var parameters = new DynamicParameters();

        var query = "WITH cte_excluded_vacancies AS (SELECT \"VacancyId\" " +
                    "FROM \"Moderation\".\"Vacancies\" " +
                    "WHERE \"ModerationStatusId\" IN (2, 3, 7)), " +
                    "cte_archived_vacancies AS (SELECT \"VacancyId\" " +
                    "FROM \"Vacancies\".\"ArchivedVacancies\") " +
                    "SELECT uv.\"VacancyName\", " +
                    "uv.\"DateCreated\", " +
                    "uv.\"Employment\", ";

        query += Enum.Parse<FilterPayTypeEnum>(VacancyCatalogInput.Filters.Pay ?? string.Empty) == FilterPayTypeEnum.NotPay
                 || VacancyCatalogInput.Filters.Employments?.Intersect(new List<FilterEmploymentTypeEnum>
                 {
                     FilterEmploymentTypeEnum.Full,
                     FilterEmploymentTypeEnum.Partial,
                     FilterEmploymentTypeEnum.ProjectWork
                 }) is not null
            ? "uv.\"Payment\", "
            : "TO_CHAR(REPLACE(\"Payment\", ' ', '')::NUMERIC(12, 2), '99 999 999 999 999 990D99'), ";

        query += "uv.\"VacancyId\", " +
                 "uv.\"VacancyText\", " +
                 "uv.\"WorkExperience\", " +
                 "uv.\"UserId\" " +
                 "FROM \"Vacancies\".\"CatalogVacancies\" AS cv " +
                 "INNER JOIN \"Vacancies\".\"UserVacancies\" AS uv " +
                 "ON cv.\"VacancyId\" = uv.\"VacancyId\" " +
                 "WHERE NOT uv.\"VacancyId\" = ANY (SELECT \"VacancyId\" FROM cte_excluded_vacancies) " +
                 "AND NOT uv.\"VacancyId\" = ANY (SELECT \"VacancyId\" FROM cte_archived_vacancies)";

        // Если фильтр занятости = полная занятость.
        if ((VacancyCatalogInput.Filters.Employments ?? new List<FilterEmploymentTypeEnum>()).Contains(FilterEmploymentTypeEnum.Full))
        {
            parameters.Add("@employment", FilterEmploymentTypeEnum.Full.GetEnumDescription());
            query += " AND \"Employment\" = @employment";
        }
        
        // Если фильтр занятости = частичная занятость.
        if ((VacancyCatalogInput.Filters.Employments ?? new List<FilterEmploymentTypeEnum>()).Contains(FilterEmploymentTypeEnum.Partial))
        {
            parameters.Add("@employment", FilterEmploymentTypeEnum.Partial.GetEnumDescription());
            query += " AND \"Employment\" = @employment";
        }
        
        // Если фильтр занятости = проектная занятость.
        if ((VacancyCatalogInput.Filters.Employments ?? new List<FilterEmploymentTypeEnum>()).Contains(FilterEmploymentTypeEnum
                .ProjectWork))
        {
            parameters.Add("@employment", FilterEmploymentTypeEnum.ProjectWork.GetEnumDescription());
            query += " AND \"Employment\" = @employment";
        }
        
        // Если фильтр опыта работы более 6 лет.
        if (Enum.Parse<FilterExperienceTypeEnum>(VacancyCatalogInput.Filters.Experience ?? string.Empty) ==
            FilterExperienceTypeEnum.ManySix)
        {
            parameters.Add("@workExperience", FilterExperienceTypeEnum.ManySix.GetEnumDescription());
            query += " AND \"WorkExperience\" = @workExperience";
        }
        
        // Если фильтр опыта работы = нет опыта.
        if (Enum.Parse<FilterExperienceTypeEnum>(VacancyCatalogInput.Filters.Experience ?? string.Empty) ==
            FilterExperienceTypeEnum.NotExperience)
        {
            parameters.Add("@workExperience", FilterExperienceTypeEnum.NotExperience.GetEnumDescription());
            query += " AND \"WorkExperience\" = @workExperience";
        }
        
        // Если фильтр имеет тип оплаты = от 1 года до 3 лет.
        if (Enum.Parse<FilterExperienceTypeEnum>(VacancyCatalogInput.Filters.Experience ?? string.Empty) ==
            FilterExperienceTypeEnum.OneThree)
        {
            parameters.Add("@workExperience", FilterExperienceTypeEnum.OneThree.GetEnumDescription());
            query += " AND \"WorkExperience\" = @workExperience";
        }
        
        // Если фильтр опыта работы = от 3 до 6 лет.
        if (Enum.Parse<FilterExperienceTypeEnum>(VacancyCatalogInput.Filters.Experience ?? string.Empty) ==
            FilterExperienceTypeEnum.ThreeSix)
        {
            parameters.Add("@workExperience", FilterExperienceTypeEnum.ThreeSix.GetEnumDescription());
            query += " AND \"WorkExperience\" = @workExperience";
        }
        
        // Если фильтр опыта работы = не имеет значения.
        if (Enum.Parse<FilterExperienceTypeEnum>(VacancyCatalogInput.Filters.Experience ?? string.Empty) ==
            FilterExperienceTypeEnum.UnknownExperience)
        {
            parameters.Add("@workExperience", FilterExperienceTypeEnum.UnknownExperience.GetEnumDescription());
            query += " AND \"WorkExperience\" = @workExperience";
        }
        
        // Если фильтр имеет тип оплаты = без оплаты.
        if (Enum.Parse<FilterPayTypeEnum>(VacancyCatalogInput.Filters.Pay ?? string.Empty) == FilterPayTypeEnum.NotPay)
        {
            parameters.Add("@payment", FilterPayTypeEnum.NotPay.GetEnumDescription());
            query += " AND \"Payment\" = @payment";
        }
        
        // Если фильтр имеет тип оплаты = есть оплата.
        if (Enum.Parse<FilterPayTypeEnum>(VacancyCatalogInput.Filters.Pay ?? string.Empty) == FilterPayTypeEnum.Pay)
        {
            query += " AND \"Payment\" <> 'Без оплаты' " +
                     "AND REPLACE(\"Payment\", ' ', '')::NUMERIC(12, 2) > 0";
        }
        
        // Если фильтр не имеет тип оплаты (не имеет значения), то передаем следующему по цепочке.
        if (Enum.Parse<FilterPayTypeEnum>(VacancyCatalogInput.Filters.Pay ?? string.Empty) == FilterPayTypeEnum.UnknownPay)
        {
            parameters.Add("@payment", FilterPayTypeEnum.UnknownPay.GetEnumDescription());
            query += " AND \"Payment\" = @payment";
        }

        // Если фильтр по дате.
        if (!isNeedOrder && Enum.Parse<FilterSalaryTypeEnum>(VacancyCatalogInput.Filters.Salary ?? string.Empty) ==
            FilterSalaryTypeEnum.Date)
        {
            isNeedOrder = true;
            query += " ORDER BY \"DateCreated\" DESC";
        }
        
        // Если фильтр по возрастанию зарплаты.
        if (!isNeedOrder && Enum.Parse<FilterSalaryTypeEnum>(VacancyCatalogInput.Filters.Salary ?? string.Empty) ==
            FilterSalaryTypeEnum.AscSalary)
        {
            isNeedOrder = true;
            
            // Исключаем без оплаты, так как это помешает сортировке по цене.
            query += " AND \"Payment\" <> 'Без оплаты' " +
                     "ORDER BY REPLACE(\"Payment\", ' ', '')::NUMERIC(12, 2)";
        }
        
        // Если фильтр по возрастанию зарплаты.
        if (!isNeedOrder && Enum.Parse<FilterSalaryTypeEnum>(VacancyCatalogInput.Filters.Salary ?? string.Empty) ==
            FilterSalaryTypeEnum.DescSalary)
        {
            isNeedOrder = true;
            
            // Исключаем без оплаты, так как это помешает сортировке по цене.
            query += " AND \"Payment\" <> 'Без оплаты' " +
                     "ORDER BY REPLACE(\"Payment\", ' ', '')::NUMERIC(12, 2) DESC";
		}

		if (VacancyCatalogInput.LastId.HasValue)
		{
			parameters.Add("@lastId", VacancyCatalogInput.LastId);

			// Применяем пагинацию.
			query += "AND cv.\"VacancyId\">@lastId ";
		}

		// TODO: Передавать с фронта будем кол-во строк, при настройке пагинации пользователем.
		parameters.Add("@countRows", VacancyCatalogInput.PaginationRows);

		query += "LIMIT @countRows";

		var result = await connection.QueryAsync<CatalogVacancyOutput>(query, parameters);

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

    #endregion
}