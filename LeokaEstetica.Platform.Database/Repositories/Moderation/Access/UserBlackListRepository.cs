using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Access;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Entities.Access;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace LeokaEstetica.Platform.Database.Repositories.Moderation.Access;

/// <summary>
/// Класс репозитория ЧС пользователей.
/// </summary>
public class UserBlackListRepository : IUserBlackListRepository
{
    private readonly PgContext _pgContext;
    private readonly ILogService _logService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    /// <param name="logService">Сервис логера.</param>
    public UserBlackListRepository(PgContext pgContext,
                                   ILogService logService)
    {
        _pgContext = pgContext;
        _logService = logService;
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
    /// Метод удаляет пользователя из ЧС.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    public async Task RemoveUserBlackListAsync(long userId)
    {
        bool IsEmailExist = await IsEmailUserExistAsync(userId);
        bool IsPhoneExist = await IsPhonelUserExistAsync(userId);

        if (IsEmailExist == false && IsPhoneExist == false)
        {
            throw new ArgumentNullException($"Пользователя {userId} нет в черном списке");
        }

        if (IsEmailExist)
        {
            var entity = await _pgContext.UserEmailBlackList.FirstAsync(x => x.UserId == userId);
            _pgContext.UserEmailBlackList.Remove(entity);
        }

        if (IsPhoneExist)
        {
            var entity = await _pgContext.UserPhoneBlackList.FirstAsync(x => x.UserId == userId);
            _pgContext.UserPhoneBlackList.Remove(entity);
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
    /// Метод проверяет наличие пользователя в ЧС по почте.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Наличие пользователя в чс</returns>
    private async Task<bool> IsEmailUserExistAsync(long userId)
    {
        var result = await _pgContext.UserEmailBlackList.AnyAsync(row => row.UserId == userId);

        return result;
    }

    /// <summary>
    /// Метод проверяет наличие пользователя в ЧС по номеру телефона.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Наличие пользователя в чс</returns>
    private async Task<bool> IsPhonelUserExistAsync(long userId)
    {
        var result = await _pgContext.UserPhoneBlackList.AnyAsync(row => row.UserId == userId);

        return result;
    }
}