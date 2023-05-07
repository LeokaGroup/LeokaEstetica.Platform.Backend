using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Access;
using LeokaEstetica.Platform.Models.Entities.Access;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Moderation.Access;

/// <summary>
/// Класс репозитория ЧС пользователей.
/// </summary>
public class UserBlackListRepository : IUserBlackListRepository
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
        var isEmailExist = await IsEmailUserExistAsync(userId);
        var isPhoneExist = await IsPhoneUserExistAsync(userId);

        if (!isEmailExist)
        {
            if (!string.IsNullOrEmpty(email))
            {
                await _pgContext.UserEmailBlackList.AddAsync(new UserEmailBlackListEntity
                {
                    UserId = userId,
                    Email = email
                });
            }
        }

        if (!isPhoneExist)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                await _pgContext.UserPhoneBlackList.AddAsync(new UserPhoneBlackListEntity
                {
                    UserId = userId,
                    PhoneNumber = phoneNumber
                });
            }
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
    /// Метод проверяет наличие пользователя в ЧС по email или phone number.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Наличие пользователя в чс по email или phone number.</returns>
    public async Task<bool> IsUserExistAsync(long userId)
    {
        var result = false;

        var emailExist = await IsEmailUserExistAsync(userId);
        var phoneExist = await IsPhoneUserExistAsync(userId);

        if (emailExist || phoneExist)
            result = true;

        return result;
    }

    /// <summary>
    /// Метод проверяет, заблокирован ли пользователь по email и phone number
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>Признак наличия пользователя в чс по email и phone number</returns>
    public async Task<bool> IsUserBlocked(long userId)
    {
        var result = false;

        var emailExist = await IsEmailUserExistAsync(userId);
        var phoneExist = await IsPhoneUserExistAsync(userId);

        if (emailExist && phoneExist)
            result = true;

        return result;
    }

    /// <summary>
    /// Метод проверяет наличие пользователя в ЧС по почте.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Наличие пользователя в чс.</returns>
    private async Task<bool> IsEmailUserExistAsync(long userId)
    {
        var result = await _pgContext.UserEmailBlackList.AnyAsync(row => row.UserId == userId);

        return result;
    }

    /// <summary>
    /// Метод проверяет наличие пользователя в ЧС по номеру телефона.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Наличие пользователя в чс.</returns>
    private async Task<bool> IsPhoneUserExistAsync(long userId)
    {
        var result = await _pgContext.UserPhoneBlackList.AnyAsync(row => row.UserId == userId);

        return result;
    }
}