using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Models.Dto.Output.User;
using LeokaEstetica.Platform.Models.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Database.Repositories.User;

/// <summary>
/// Класс реализует методы репозитория пользователей.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly PgContext _pgContext;
    private readonly ILogger<UserRepository> _logger;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    /// <param name="logger">Сервис логгера.</param>
    public UserRepository(PgContext pgContext, 
        ILogger<UserRepository> logger)
    {
        _pgContext = pgContext;
        _logger = logger;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод сохраняет нового пользователя в базу.
    /// </summary>
    /// <param name="user">Данные пользователя для добавления.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> AddUserAsync(UserEntity user)
    {
        await _pgContext.Users.AddAsync(user);
        await _pgContext.SaveChangesAsync();

        return user.UserId;
    }

    /// <summary>
    /// Метод находит пользователя по его UserId.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Данные пользователя.</returns>
    public async Task<UserEntity> GetUserByUserIdAsync(long userId)
    {
        var result = await _pgContext.Users
            .FirstOrDefaultAsync(u => u.UserId == userId);

        return result;
    }

    /// <summary>
    /// Метод проверет существование пользователя по email в базе.
    /// </summary>
    /// <param name="email">Email пользователя.</param>
    /// <returns>Флаг проверки.</returns>
    public async Task<bool> CheckUserByEmailAsync(string email)
    {
        var result = await _pgContext.Users
            .AnyAsync(u => u.Email.Equals(email));

        return result;
    }

    /// <summary>
    /// Метод запишет код подтверждения пользователю.
    /// </summary>
    /// <param name="code">Код подтверждения, который мы отправили пользователю на почту.</param>
    /// <param name="userId">UserId.</param>
    public async Task SetConfirmAccountCodeAsync(Guid code, long userId)
    {
        var user = await GetUserByUserIdAsync(userId);

        if (user is null)
        {
            throw new NotFoundUserByIdException(userId);
        }
        
        user.ConfirmEmailCode = code;
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод подтверждает аккаунт пользователя по коду, который ранее был отправлен пользователю на почту и записан в БД.
    /// </summary>
    /// <param name="code">Код подтверждения.</param>
    /// <returns>Статус подтверждения.</returns>
    public async Task<bool> ConfirmAccountAsync(Guid code)
    {
        var user = await _pgContext.Users.FirstOrDefaultAsync(u => u.ConfirmEmailCode.Equals(code));

        if (user is null)
        {
            throw new InvalidOperationException($"Не удалось подтвердить почту пользователя по коду {code}!");
        }

        user.EmailConfirmed = true;
        await _pgContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Метод получает хэш пароля для проверки пользователя.
    /// </summary>
    /// <param name="email">Почта.</param>
    /// <returns>Хэш пароля.</returns>
    public async Task<string> GetPasswordHashByEmailAsync(string email)
    {
        var result = await _pgContext.Users
            .Where(u => u.Email.Equals(email))
            .Select(u => u.PasswordHash)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод находит Id пользователя по его почте.
    /// </summary>
    /// <param name="account">Почта пользователя.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> GetUserByEmailAsync(string account)
    {
        try
        {
            var result = await _pgContext.Users
                .Where(u => u.Email.Equals(account))
                .Select(u => u.UserId)
                .FirstOrDefaultAsync();

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
    
    /// <summary>
    /// Метод получает код пользователя по его почте.
    /// </summary>
    /// <param name="email">Почта.</param>
    /// <returns>Хэш пароля.</returns>
    public async Task<Guid> GetUserCodeByEmailAsync(string email)
    {
        var result = await _pgContext.Users
            .Where(u => u.Email.Equals(email))
            .Select(u => u.UserCode)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает код пользователя по его VkUserId.
    /// </summary>
    /// <param name="vkUserId">VkUserId пользователя.</param>
    /// <returns>Хэш пароля.</returns>
    public async Task<Guid> GetUserCodeByVkUserIdAsync(long vkUserId)
    {
        var result = await _pgContext.Users
            .Where(u => u.VkUserId == vkUserId)
            .Select(u => u.UserCode)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает недостающую информацию профиля по UserId.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Основные данные профиля.</returns>
    public async Task<UserInfoOutput> GetUserPhoneEmailByUserIdAsync(long userId)
    {
        var result = await _pgContext.Users
            .Where(u => u.UserId == userId)
            .Select(u => new UserInfoOutput
            {
                PhoneNumber = u.PhoneNumber,
                Email = u.Email
            })
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод сохраняет телефон и почту пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="phone">Номер телефона.</param>
    public async Task SaveUserPhoneAsync(long userId, string phone)
    {
        var user = await GetUserByUserIdAsync(userId);
        user.PhoneNumber = phone;
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод находит логин и почту пользователей по почте или логину пользователя.
    /// </summary>
    /// <param name="searchText">Текст, по которому надо искать.</param>
    /// <returns>Список пользователей.</returns>
    public async Task<List<UserEntity>> GetUserByEmailOrLoginAsync(string searchText)
    {
        var result = await _pgContext.Users
            .Where(u => u.Email.Contains(searchText) 
                        || u.Login.Contains(searchText))
            .Select(u => new UserEntity
            {
                Email = u.Email,
                Login = u.Login,
                UserId = u.UserId
            })
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод находит Id пользователя по почте или логину пользователя.
    /// </summary>
    /// <param name="searchText">Текст, по которому надо искать.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> GetUserIdByEmailOrLoginAsync(string searchText)
    {
        var result = await _pgContext.Users
            .Where(u => u.Email.Contains(searchText) 
                        || u.Login.Contains(searchText))
            .Select(u => u.UserId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает словарь кодов пользователей.
    /// </summary>
    /// <returns>Словарь кодов пользователей.</returns>
    public async Task<Dictionary<long, Guid>> GetUsersCodesAsync()
    {
        var result = await _pgContext.Users.ToDictionaryAsync(k => k.UserId, v => v.UserCode);

        return result;
    }

    /// <summary>
    /// Метод получает список пользователей.
    /// </summary>
    /// <returns>Список пользователей.</returns>
    public async Task<List<UserEntity>> GetAllAsync()
    {
        var result = await _pgContext.Users.ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод находит Id пользователя по его коду.
    /// </summary>
    /// <param name="userCode">Код пользователя.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> GetUserIdByCodeAsync(string userCode)
    {
        var result = await _pgContext.Users
            .Where(u => u.UserCode.Equals(userCode))
            .Select(u => u.UserId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод находит Id пользователя по его Email.
    /// </summary>
    /// <param name="email">Email пользователя.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> GetUserIdByEmailAsync(string email)
    {
        var result = await _pgContext.Users
            .Where(u => u.Email.Equals(email))
            .Select(u => u.UserId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод находит Id пользователя по его номеру телефона.
    /// </summary>
    /// <param name="phoneNumber">Номер телефона пользователя.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> GetUserIdByPhoneNumberAsync(string phoneNumber)
    {
        var result = await _pgContext.Users
            .Where(u => u.PhoneNumber.Equals(phoneNumber))
            .Select(u => u.UserId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод находит Id пользователя по его логину.
    /// </summary>
    /// <param name="phoneNumber">Логин пользователя.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> GetUserIdByLoginAsync(string login)
    {
        var result = await _pgContext.Users
            .Where(u => u.Login.Equals(login))
            .Select(u => u.UserId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод проверет существование пользователя по VkUserId в базе.
    /// </summary>
    /// <param name="userId">VkUserId пользователя.</param>
    /// <returns>Флаг проверки.</returns>
    public async Task<bool> CheckUserByVkUserIdAsync(long userId)
    {
        var result = await _pgContext.Users
            .AnyAsync(u => u.VkUserId == userId);

        return result;
    }

    /// <summary>
    /// Метод находит Id пользователя по vk id.
    /// </summary>
    /// <param name="vkId">Id вконтакте.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> GetUserIdByVkIdAsync(long vkId)
    {
        var result = await _pgContext.Users
            .Where(u => u.VkUserId == vkId)
            .Select(u => u.UserId)
            .FirstOrDefaultAsync();

        return result;
    }
    
    /// Метод проставляет пользователям метку к удалению аккаунтов.
    /// </summary>
    /// <param name="users">Список пользователей, которых предупредим.</param>
    public async Task SetMarkDeactivateAccountsAsync(List<UserEntity> users)
    {
        _pgContext.Users.UpdateRange(users);
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод удаляет аккаунты пользователей.
    /// </summary>
    /// <param name="users">Список пользователей, которых предупредим.</param>
    public async Task DeleteDeactivateAccountsAsync(List<UserEntity> users)
    {
        _pgContext.Users.RemoveRange(users);
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод находит Id пользователя по его Id анкеты.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    /// <returns>Id пользователя.</returns>
    public async Task<long> GetUserIdByProfileInfoIdAsync(long profileInfoId)
    {
        var result = await _pgContext.ProfilesInfo
            .Where(u => u.ProfileInfoId == profileInfoId)
            .Select(u => u.UserId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод находит аккаунт пользователя по его Id анкеты.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Аккаунт пользователя.</returns>
    public async Task<string> GetUserAccountByUserIdAsync(long userId)
    {
        var result = await _pgContext.Users
            .Where(u => u.UserId == userId)
            .Select(u => u.Email)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод находит Id анкеты по Id пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Id анкеты.</returns>
    public async Task<long> GetProfileInfoIdByUserIdAsync(long userId)
    {
        var result = await _pgContext.ProfilesInfo
            .Where(u => u.UserId == userId)
            .Select(u => u.ProfileInfoId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает дату начала подписки и дату окончания подписки пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Дата начала и дата окончания подписки.</returns>
    public async Task<(DateTime? StartDate, DateTime? EndDate)> GetUserSubscriptionUsedDateAsync(long userId)
    {
        (DateTime? StartDate, DateTime? EndDate) result = (null, null);
        var item = await _pgContext.Users
            .Where(u => u.UserId == userId)
            .Select(u => new
            {
                u.SubscriptionStartDate,
                u.SubscriptionEndDate
            })
            .FirstOrDefaultAsync();

        if (item is null)
        {
            return result;
        }

        result.StartDate = item.SubscriptionStartDate;
        result.EndDate = item.SubscriptionEndDate;

        return result;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}