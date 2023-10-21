using System.Runtime.CompilerServices;
using AutoMapper;
using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Models.Entities.User;
using LeokaEstetica.Platform.Redis.Abstractions.User;
using LeokaEstetica.Platform.Redis.Consts;
using LeokaEstetica.Platform.Redis.Extensions;
using Microsoft.Extensions.Caching.Distributed;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Redis.Services.User;

/// <summary>
/// Класс реализует методы сервиса кэша пользователей.
/// </summary>
internal sealed class UserRedisService : IUserRedisService
{
    private readonly IDistributedCache _redisCache;
    private readonly IMapper _mapper;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="redisCache">Кэш редиса.</param>
    /// <param name="mapper">Маппер.</param>
    public UserRedisService(IDistributedCache redisCache, 
        IMapper mapper)
    {
        _redisCache = redisCache;
        _mapper = mapper;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод добавляет в кэш Id и токен пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task AddUserTokenAndUserIdCacheAsync(long userId, string token)
    {
        await _redisCache.SetStringAsync(token, ProtoBufExtensions.Serialize(userId.ToString()),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            });
    }

    /// <summary>
    /// Метод добавляет в кэш пользователей, аккаунты которых нужно удалить и все их данные.
    /// </summary>
    /// <param name="users">Список пользователей.</param>
    public async Task AddMarkDeactivateUserAccountsAsync(List<UserEntity> users)
    {
        if (!users.Any())
        {
            return;
        }

        var mapItems = _mapper.Map<List<UserActivityMarkDeactivate>>(users);

        await _redisCache.SetStringAsync(CacheKeysConsts.DEACTIVATE_USER_ACCOUNTS,
            ProtoBufExtensions.Serialize(mapItems),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });
    }

    /// <summary>
    /// Метод получает из кэша пользователей, аккаунты которых нужно удалить и все их данные.
    /// </summary>
    /// <param name="key">Ключ для получения списка аккаунтов.</param>
    /// <returns>Список пользователей для удаления их аккаунтов.</returns>
    public async Task<List<UserEntity>> GetMarkDeactivateUserAccountsAsync(string key)
    {
        var items = await _redisCache.GetStringAsync(key);

        // Нет аккаунтов для удаления.
        if (string.IsNullOrEmpty(items))
        {
            return new List<UserEntity>();
        }

        var redisResult = ProtoBufExtensions.Deserialize<List<UserActivityMarkDeactivate>>(items);
        
        if (redisResult is null || !redisResult.Any())
        {
            throw new InvalidOperationException(
                "Не удалось получить список аккаунтов пользователей из кэша для удаления.");
        }
        
        var result = _mapper.Map<List<UserEntity>>(redisResult);

        return result;
    }

    /// <summary>
    /// Метод добавляет в кэш данные для восстановления пароля пользователя.
    /// </summary>
    /// <param name="code">Код для отправки его в ссылке.</param>
    /// <param name="userId">Id пользователя.</param>
    public async Task AddRestoreUserDataCacheAsync(string code, long userId)
    {
        await _redisCache.SetStringAsync(userId.ToString(), ProtoBufExtensions.Serialize(code),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
            });
    }

    /// <summary>
    /// Метод получает код восстановления пароля.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Признак успешной проверки.</returns>
    public async Task<bool> GetRestoreUserDataCacheAsync(long userId)
    {
        var result = await _redisCache.GetStringAsync(userId.ToString());

        // Нет аккаунтов для удаления.
        if (string.IsNullOrEmpty(result))
        {
            return false;
        }

        var redisResult = ProtoBufExtensions.Deserialize<string>(result);
        
        if (string.IsNullOrWhiteSpace(redisResult))
        {
            throw new InvalidOperationException(
                "Не удалось получить код из кэша для проверки восстановления пароля пользователя.");
        }

        return true;
    }

    /// <summary>
    /// Метод удаляет из кэша пользователей, которых ранее помечали к удалению.
    /// К этому моменту они уже удалены из БД, поэтому из кэша надо удалить тоже.
    /// </summary>
    public async Task DeleteMarkDeactivateUserAccountsAsync()
    {
        await _redisCache.RemoveAsync(CacheKeysConsts.DEACTIVATE_USER_ACCOUNTS);
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}