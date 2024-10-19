using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Access;
using LeokaEstetica.Platform.Models.Entities.Access;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LeokaEstetica.Platform.Database.Repositories.Moderation.Access;

/// <summary>
/// Класс репозитория ЧС пользователей.
/// </summary>
internal sealed class UserBlackListRepository : BaseRepository, IUserBlackListRepository
{
    private readonly PgContext _pgContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    /// <param name="connectionProvider">Провайдер БД.</param>
    public UserBlackListRepository(PgContext pgContext,
        IConnectionProvider connectionProvider) : base(connectionProvider)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод добавляет пользователя в ЧС.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="email">Почта для блока.</param>
    /// <param name="phoneNumber">Номер телефона для блока.</param>
    /// <param name="vkUserId">Id пользователя в системе ВКонтакте.</param>
    public async Task AddUserBlackListAsync(long userId, string? email, string? phoneNumber, string? vkUserId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        using var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId);
        
        try
        {
            if (!string.IsNullOrEmpty(email))
            {
                parameters.Add("@email", email);

                var insertUserEmailBlackQuery = "INSERT INTO access.user_email_black_list (user_id, email) " +
                                                "VALUES (@userId, @email)";
                
                await connection.ExecuteAsync(insertUserEmailBlackQuery, parameters);
            }

            if (!string.IsNullOrEmpty(phoneNumber))
            { 
                parameters.Add("@phoneNumber", phoneNumber);

                var insertUserPhoneNumberBlackQuery = "INSERT INTO access.user_phone_black_list (user_id, phone_number) " +
                                                      "VALUES (@userid, @phoneNumber)";

                await connection.ExecuteAsync(insertUserPhoneNumberBlackQuery, parameters);
            }

            if (!string.IsNullOrEmpty(vkUserId))
            {
                parameters.Add("@vkUserId", vkUserId);

                var insertUserVkBlackQuery = "INSERT INTO access.user_vk_black_list (user_id, vk_user_id) " +
                                             "VALUES (@userId, @vkUserId)";

                await connection.ExecuteAsync(insertUserVkBlackQuery, parameters);
            }

            transaction.Commit();
        }

        catch
        {
            transaction.Rollback();

            throw;
        }
    }

    /// <summary>
    /// Метод получает список пользователей в ЧС.
    /// </summary>
    /// <returns>Список пользователей в ЧС.</returns>
    public async Task<(IEnumerable<UserEmailBlackListEntity>, IEnumerable<UserPhoneBlackListEntity>)>
        GetUsersBlackListAsync()
    {
        (IEnumerable<UserEmailBlackListEntity>, IEnumerable<UserPhoneBlackListEntity>) result;

        result.Item1 = await _pgContext.UserEmailBlackList.ToListAsync();
        result.Item2 = await _pgContext.UserPhoneBlackList.ToListAsync();

        return result;
    }
}