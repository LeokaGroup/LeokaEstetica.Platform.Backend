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
        using var connection = await ConnectionProvider.GetConnectionAsync();
        using var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            var insertVacancyParameters = new DynamicParameters();
            insertVacancyParameters.Add("@dateCreated", DateTime.UtcNow);
            insertVacancyParameters.Add("@vacancyName", vacancyInput.VacancyName);
            insertVacancyParameters.Add("@vacancyText", vacancyInput.VacancyText);
            insertVacancyParameters.Add("@workExperience", vacancyInput.WorkExperience);
            insertVacancyParameters.Add("@employment", vacancyInput.Employment);
            insertVacancyParameters.Add("@payment", vacancyInput.Payment);
            insertVacancyParameters.Add("@userId", userId);
            insertVacancyParameters.Add("@demands", vacancyInput.Demands);
            insertVacancyParameters.Add("@conditions", vacancyInput.Conditions);

            var insertVacancyQuery = "INSERT INTO \"Vacancies\".\"UserVacancies\" (" +
                                     "\"DateCreated\", " +
                                     "\"VacancyName\", " +
                                     "\"VacancyText\", " +
                                     "\"WorkExperience\", " +
                                     "\"Employment\", " +
                                     "\"Payment\", " +
                                     "\"UserId\", " +
                                     "\"Demands\", " +
                                     "\"Conditions\") " +
                                     "VALUES (" +
                                     "@dateCreated, " +
                                     "@vacancyName, " +
                                     "@vacancyText, " +
                                     "@workExperience, " +
                                     "@employment, " +
                                     "@payment, " +
                                     "@userId, " +
                                     "@demands, " +
                                     "@conditions) " +
                                     "RETURNING \"VacancyId\"";

            var addedVacancyId = await connection.ExecuteScalarAsync<long>(insertVacancyQuery,
                insertVacancyParameters);
            
            // Добавляем вакансию в таблицу статусов вакансий. Проставляем новой вакансии статус "На модерации".
            var insertVacancyStatusParameters = new DynamicParameters();
            insertVacancyStatusParameters.Add("@vacancyId", addedVacancyId);
            insertVacancyStatusParameters.Add("@statusName", VacancyStatusNameEnum.Moderation.GetEnumDescription());
            insertVacancyStatusParameters.Add("@statusSysName", VacancyStatusNameEnum.Moderation.ToString());

            var insertVacancyStatusQuery = "INSERT INTO \"Vacancies\".\"VacancyStatuses\" (" +
                                           "\"VacancyStatusSysName\", " +
                                           "\"VacancyStatusName\", " +
                                           "\"VacancyId\") " +
                                           "VALUES (" +
                                           "@statusSysName, " +
                                           "@statusName, " +
                                           "@vacancyId)";

            await connection.ExecuteAsync(insertVacancyStatusQuery, insertVacancyStatusParameters);

            transaction.Commit();
            
            var parameters = new DynamicParameters();
            parameters.Add("@vacancyId", addedVacancyId);

            var query = "SELECT * " +
                        "FROM \"Vacancies\".\"UserVacancies\" " +
                        "WHERE \"VacancyId\" = @vacancyId";

            var result = await connection.QuerySingleOrDefaultAsync<UserVacancyEntity>(query, parameters);

            return result;
        }
        
        catch
        {
            transaction.Rollback();
            throw;
        }
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
    public async Task<CatalogVacancyResultOutput> GetCatalogVacanciesAsync(VacancyCatalogInput vacancyCatalogInput)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();

        var query = "SELECT c.\"CatalogVacancyId\", " +
                    "u.\"VacancyId\", " +
                    " u.\"VacancyName\", " +
                    " u.\"DateCreated\", " +
                    " u.\"VacancyText\", " +
                    " u.\"UserId\" " +
                    "FROM \"Vacancies\".\"CatalogVacancies\" AS c " +
                    "INNER JOIN \"Vacancies\".\"UserVacancies\" AS u ON c.\"VacancyId\" = u.\"VacancyId\" " +
                    "LEFT JOIN \"Moderation\".\"Vacancies\" AS p ON u.\"VacancyId\" = p.\"VacancyId\" " +
                    "INNER JOIN \"Subscriptions\".\"UserSubscriptions\" AS u0 ON u.\"UserId\" = u0.\"UserId\" " +
                    "INNER JOIN \"Subscriptions\".\"Subscriptions\" AS s ON u0.\"SubscriptionId\" = s.\"ObjectId\" " +
                    "WHERE " +
                    "(NOT (EXISTS ( " +
                    "SELECT 1 " +
                    "FROM \"Vacancies\".\"ArchivedVacancies\" AS a " +
                    "WHERE a.\"VacancyId\" = u.\"VacancyId\"))) " +
                    "AND (p.\"ModerationStatusId\" NOT IN (2, 3, 6, 7) " +
                    "AND (p.\"ModerationStatusId\" IS NOT NULL)) ";

        vacancyCatalogInput.Filters ??= new FilterVacancyInput();

        // Фильтр по занятости.
        if (!string.IsNullOrWhiteSpace(vacancyCatalogInput.Filters.EmploymentsValues))
        {
            // EmploymentsValues разделены запятой в строке с фронта, поэтому можем через IN.
            parameters.Add("@employments", vacancyCatalogInput.Filters.EmploymentsValues);
            query += "AND u.\"Employment\" IN (@employments) ";
        }

        // Фильтр по опыту работы.
        if (!string.IsNullOrWhiteSpace(vacancyCatalogInput.Filters.Experience))
        {
            parameters.Add("@workExperience", vacancyCatalogInput.Filters.Experience);
            query += "AND u.\"WorkExperience\" = @workExperience ";
        }

        // Фильтр по оплате.
        if (!string.IsNullOrWhiteSpace(vacancyCatalogInput.Filters.Pay))
        {
            // Без разницы.
            if (Enum.Parse<FilterPayTypeEnum>(vacancyCatalogInput.Filters.Pay) == FilterPayTypeEnum.UnknownPay)
            {
                query += "AND u.\"Payment\" = 'Без оплаты' ";
            }
            
            // Есть оплата.
            if (Enum.Parse<FilterPayTypeEnum>(vacancyCatalogInput.Filters.Pay) == FilterPayTypeEnum.Pay)
            {
                query += "AND u.\"Payment\" = REPLACE(u.\"Payment\", 'Без оплаты', 0)::NUMERIC(12, 2) > 0 ";
            }
            
            // Без оплата.
            if (Enum.Parse<FilterPayTypeEnum>(vacancyCatalogInput.Filters.Pay) == FilterPayTypeEnum.NotPay)
            {
                query += "AND u.\"Payment\" = REPLACE(u.\"Payment\", 'Без оплаты', 0)::NUMERIC(12, 2) = 0 ";
            }
        }
        
        if (vacancyCatalogInput.LastId.HasValue)
        {
            parameters.Add("@lastId", vacancyCatalogInput.LastId);
	        
            // Применяем пагинацию.
            query += "AND c.\"CatalogVacancyId\" > @lastId ";
        }
        
        // Поисковой поисковый запрос.
        if (!string.IsNullOrWhiteSpace(vacancyCatalogInput.SearchText))
        {
            parameters.Add("@searchText", string.Concat(vacancyCatalogInput.SearchText, "%"));
            query+= " AND (u.\"VacancyName\" ILIKE @searchText " +
                    " OR u.\"VacancyText\" ILIKE @searchText) ";
        }
        
        // TODO: Передавать с фронта будем кол-во строк, при настройке пагинации пользователем.
        parameters.Add("@countRows", 20);
        
        // Фильтр по дате (возрастанию даты).
        if (!string.IsNullOrWhiteSpace(vacancyCatalogInput.Filters.Salary)
            && Enum.Parse<FilterSalaryTypeEnum>(vacancyCatalogInput.Filters.Salary) == FilterSalaryTypeEnum.Date)
        {
            // Фильтр по дате.
            query += "ORDER BY c.\"CatalogVacancyId\", u.\"DateCreated\" ";
        }

        // По убыванию даты и по доп.фильтрам сортировки.
        else
        {
            // Фильтр по возрастанию оплаты.
            if (!string.IsNullOrWhiteSpace(vacancyCatalogInput.Filters.Salary)
                && Enum.Parse<FilterSalaryTypeEnum>(vacancyCatalogInput.Filters.Salary) ==
                FilterSalaryTypeEnum.AscSalary)
            {
                // Фильтр по дате и по возрастанию оплаты.
                query += "ORDER BY c.\"CatalogVacancyId\", " +
                         "u.\"DateCreated\" DESC, " +
                         "REPLACE(u.\"Payment\", 'Без оплаты', 0)::NUMERIC(12, 2) ";
            }
            
            // Фильтр по убыванию оплаты.
            if (!string.IsNullOrWhiteSpace(vacancyCatalogInput.Filters.Salary)
                && Enum.Parse<FilterSalaryTypeEnum>(vacancyCatalogInput.Filters.Salary) ==
                FilterSalaryTypeEnum.DescSalary)
            {
                // Фильтр по дате и по убыванию оплаты.
                query += "ORDER BY c.\"CatalogVacancyId\", " +
                         "u.\"DateCreated\" DESC, " +
                         "REPLACE(u.\"Payment\", 'Без оплаты', 0)::NUMERIC(12, 2) DESC ";
            }
        }

        query += "LIMIT @countRows";

        var items = (await connection.QueryAsync<CatalogVacancyOutput>(query, parameters))?.AsList();
        
        // Нет данных, значит каталог проектов еще пуст.
        if (items is null || items.Count == 0)
        {
            return new CatalogVacancyResultOutput
            {
                CatalogVacancies = new List<CatalogVacancyOutput>(),
                LastId = null,
                Total = 0
            };
        }
        
        var calcCountQuery =
            "SELECT COUNT (c.\"CatalogProjectId\") " +
            "FROM \"Projects\".\"CatalogProjects\" AS c " +
            "INNER JOIN \"Projects\".\"UserProjects\" AS u ON c.\"ProjectId\" = u.\"ProjectId\" " +
            "LEFT JOIN \"Moderation\".\"Projects\" AS p ON u.\"ProjectId\" = p.\"ProjectId\" " +
            "INNER JOIN \"Subscriptions\".\"UserSubscriptions\" AS u0 ON u.\"UserId\" = u0.\"UserId\" " +
            "INNER JOIN \"Subscriptions\".\"Subscriptions\" AS s ON u0.\"SubscriptionId\" = s.\"ObjectId\" " +
            "INNER JOIN \"Projects\".\"UserProjectsStages\" AS u1 ON u.\"ProjectId\" = u1.\"ProjectId\" " +
            "INNER JOIN \"Projects\".\"UserProjects\" AS u2 ON c.\"ProjectId\" = u2.\"ProjectId\" " +
            "INNER JOIN \"Projects\".\"ProjectStages\" AS p0 ON p0.\"StageId\" = u1.\"StageId\" " +
            "WHERE " +
            "(NOT (EXISTS ( " +
            "SELECT 1 " +
            "FROM \"Projects\".\"ArchivedProjects\" AS a " +
            "WHERE a.\"ProjectId\" = u.\"ProjectId\")) " +
            "AND u.\"IsPublic\") " +
            "AND (p.\"ModerationStatusId\" NOT IN (2, 3, 6, 7) " +
            "AND (p.\"ModerationStatusId\" IS NOT NULL)) ";
        
        // Всего записей в каталоге - нужно для пагинации фронта.
        var calcCount = await connection.ExecuteScalarAsync<long>(calcCountQuery);

        var result = new CatalogVacancyResultOutput
        {
            CatalogVacancies = items,
            Total = calcCount > 0 ? calcCount : items.Count,
            LastId = items.LastOrDefault()?.CatalogVacancyId
        };

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