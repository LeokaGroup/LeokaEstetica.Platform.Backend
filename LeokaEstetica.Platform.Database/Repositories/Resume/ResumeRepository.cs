using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.Resume;
using LeokaEstetica.Platform.Models.Dto.Output.Resume;
using LeokaEstetica.Platform.Models.Entities.Profile;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Resume;

/// <summary>
/// Класс реализует методы репозитория базы резюме.
/// </summary>
internal sealed class ResumeRepository : BaseRepository, IResumeRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public ResumeRepository(PgContext pgContext,
        IConnectionProvider connectionProvider)
        : base(connectionProvider)
    {
        _pgContext = pgContext;
    }

    #region Публичные методы.


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
    /// Метод получает анкету пользователя по ее Id.
    /// </summary>
    /// <param name="resumeId">Id анкеты пользователя.</param>
    /// <returns>Данные анкеты.</returns>
    public async Task<UserInfoOutput> GetResumeAsync(long resumeId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@resumeId", resumeId);

        var query = "SELECT pi.\"ProfileInfoId\", " +
                    "pi.\"LastName\", " +
                    "pi.\"FirstName\", " +
                    "pi.\"Patronymic\", " +
                    "pi.\"IsShortFirstName\", " +
                    "pi.\"Telegram\", " +
                    "pi.\"WhatsApp\", " +
                    "pi.\"Vkontakte\", " +
                    "pi.\"OtherLink\", " +
                    "pi.\"Aboutme\", " +
                    "pi.\"Job\", " +
                    "pi.\"UserId\", " +
                    "pi.\"WorkExperience\" " +
                    "FROM \"Profile\".\"ProfilesInfo\" AS pi " +
                    "WHERE pi.\"ProfileInfoId\" NOT IN (SELECT \"ProfileInfoId\" " +
                    "FROM \"Moderation\".\"Resumes\" " +
                    "WHERE \"ModerationStatusId\" IN (2, 3)) " +
                    "AND pi.\"LastName\" <> '' " +
                    "AND pi.\"LastName\" <> '' " +
                    "AND pi.\"Job\" <> '' " +
                    "AND pi.\"Aboutme\" <> '' " +
                    "AND pi.\"UserId\" = ANY(SELECT ui.\"UserId\" " +
                    "FROM \"Profile\".\"UserIntents\" AS ui " +
                    "WHERE ui.\"UserId\" = pi.\"UserId\") " +
                    "AND pi.\"UserId\" = ANY(SELECT us.\"UserId\" " +
                    "FROM \"Profile\".\"UserSkills\" AS us " +
                    "WHERE us.\"UserId\" = pi.\"UserId\") " +
                    "AND pi.\"ProfileInfoId\" = @resumeId ";

        var result = await connection.QueryFirstOrDefaultAsync<UserInfoOutput>(query, parameters);

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

    /// <inheritdoc />
    public async Task<PaginationResumeOutput?> GetPaginationResumesAsync(long count, long? lastId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@count", count);

        var query = "SELECT pi.\"ProfileInfoId\", " +
                    "pi.\"LastName\", " +
                    "pi.\"FirstName\", " +
                    "pi.\"Patronymic\", " +
                    "pi.\"IsShortFirstName\", " +
                    "pi.\"Telegram\", " +
                    "pi.\"WhatsApp\", " +
                    "pi.\"Vkontakte\", " +
                    "pi.\"OtherLink\", " +
                    "pi.\"Aboutme\", " +
                    "pi.\"Job\", " +
                    "pi.\"UserId\", " +
                    "pi.\"WorkExperience\" " +
                    "FROM \"Profile\".\"ProfilesInfo\" AS pi " +
                    "WHERE pi.\"ProfileInfoId\" NOT IN (SELECT \"ProfileInfoId\" " +
                    "FROM \"Moderation\".\"Resumes\" " +
                    "WHERE \"ModerationStatusId\" IN (2, 3)) " +
                    "AND pi.\"LastName\" <> '' " +
                    "AND pi.\"LastName\" <> '' " +
                    "AND pi.\"Job\" <> '' " +
                    "AND pi.\"Aboutme\" <> '' " +
                    "AND pi.\"UserId\" = ANY(SELECT ui.\"UserId\" " +
                    "FROM \"Profile\".\"UserIntents\" AS ui " +
                    "WHERE ui.\"UserId\" = pi.\"UserId\") " +
                    "AND pi.\"UserId\" = ANY(SELECT us.\"UserId\" " +
                    "FROM \"Profile\".\"UserSkills\" AS us " +
                    "WHERE us.\"UserId\" = pi.\"UserId\") ";

        if (lastId.HasValue)
        {
            parameters.Add("@lastId", lastId);
            query += "AND pi.\"ProfileInfoId\" < @lastId ";
        }

        query += "ORDER BY pi.\"ProfileInfoId\" DESC " +
                 "LIMIT @count";
                 
        var items = await connection.QueryAsync<UserInfoOutput>(query, parameters);

        var queryTotalRows = "SELECT COUNT (pi.\"ProfileInfoId\") " +
                             "FROM \"Profile\".\"ProfilesInfo\" AS pi " +
                             "WHERE pi.\"ProfileInfoId\" NOT IN (SELECT \"ProfileInfoId\" " +
                             "FROM \"Moderation\".\"Resumes\" " +
                             "WHERE \"ModerationStatusId\" IN (2, 3)) " +
                             "AND pi.\"LastName\" <> '' " +
                             "AND pi.\"LastName\" <> '' " +
                             "AND pi.\"Job\" <> '' " +
                             "AND pi.\"Aboutme\" <> '' " +
                             "AND pi.\"UserId\" = ANY(SELECT ui.\"UserId\" " +
                             "FROM \"Profile\".\"UserIntents\" AS ui " +
                             "WHERE ui.\"UserId\" = pi.\"UserId\") " +
                             "AND pi.\"UserId\" = ANY(SELECT us.\"UserId\" " +
                             "FROM \"Profile\".\"UserSkills\" AS us " +
                             "WHERE us.\"UserId\" = pi.\"UserId\") ";

        var totalRows = await connection.ExecuteScalarAsync<long>(queryTotalRows);

        var result = new PaginationResumeOutput
        {
            Resumes = items.AsList(),
            Total = totalRows
        };

        return result;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}