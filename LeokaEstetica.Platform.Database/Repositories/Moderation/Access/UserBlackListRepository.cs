using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Access;
using LeokaEstetica.Platform.Models.Entities.Access;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Moderation.Access;

/// <summary>
/// Класс репозитория ЧС пользователей.
/// </summary>
internal sealed class UserBlackListRepository : IUserBlackListRepository
{
    private readonly PgContext _pgContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public UserBlackListRepository(PgContext pgContext)
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
}