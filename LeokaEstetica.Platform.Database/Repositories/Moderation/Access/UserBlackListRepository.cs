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
    /// <param name="email">Почта для блока..</param>
    /// <param name="phoneNumber">Номер телефона для блока.</param>
    public async Task AddUserBlackListAsync(long userId, string email, string phoneNumber)
    {
        if (!string.IsNullOrEmpty(email))
        {
            await _pgContext.UserEmailBlackList.AddAsync(new UserEmailBlackListEntity
            {
                UserId = userId,
                Email = email
            });
        }

        if (!string.IsNullOrEmpty(phoneNumber))
        {
            await _pgContext.UserPhoneBlackList.AddAsync(new UserPhoneBlackListEntity
            {
                UserId = userId,
                PhoneNumber = phoneNumber
            });
        }

        await _pgContext.SaveChangesAsync();
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

    /// <summary>
    /// Метод удаляет пользователя из ЧС.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="email">Почта для блока..</param>
    /// <param name="phoneNumber">Номер телефона для блока.</param>
    /// <param name="vkUserId">Id пользователя в системе ВКонтакте.</param>
    public async Task RemoveUserBlackListAsync(long userId, string? email, string? phoneNumber, long? vkUserId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        using var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId);

            if (!string.IsNullOrWhiteSpace(email))
            {
                parameters.Add("@email", email);

                var removeEmailBlackQuery = "DELETE FROM access.user_email_black_list " +
                                            "WHERE user_id = @userId " +
                                            "AND email = @email";

                await connection.ExecuteAsync(removeEmailBlackQuery, parameters);
            }

            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                parameters.Add("@phoneNumber", phoneNumber);

                var removePhoneBlackQuery = "DELETE FROM access.user_phone_black_list " +
                                                "WHERE user_id = @userId " +
                                                "AND phone_number = @phoneNumber";

                await connection.ExecuteAsync(removePhoneBlackQuery, parameters);
            }

            if (vkUserId.HasValue && vkUserId.Value > 0)
            {
                parameters.Add("@vkUserId", vkUserId);

                var removeVkIdBlackQuery = "DELETE FROM access.user_vk_black_list " +
                                           "WHERE user_id = @userId " +
                                           "AND vk_user_id = @vkUserId";

                await connection.ExecuteAsync(removeVkIdBlackQuery, parameters);
            }

            transaction.Commit();
        }

        catch
        {
            transaction.Rollback();

            throw;
        }
    }
}