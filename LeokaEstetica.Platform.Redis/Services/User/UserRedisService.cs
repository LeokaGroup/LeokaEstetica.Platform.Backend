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
    /// Метод получает Id пользователя из кэша.
    /// </summary>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Id пользователя из кэша.</returns>
    public async Task<string> GetUserIdCacheAsync(string token)
    {
        var redisResult = await _redisCache.GetStringAsync(token);
    
        if (string.IsNullOrEmpty(redisResult))
        {
            throw new InvalidOperationException($"Не удалось получить Id пользователя из кэша. Токен: {token}");
        }
        
        var result = ProtoBufExtensions.Deserialize<string>(redisResult);

        if (string.IsNullOrEmpty(result))
        {
            throw new InvalidOperationException($"Не удалось получить Id пользователя из кэша. Токен: {token}");
        }
            
        // Данные нашли, продлеваем время жизни ключа.
        await _redisCache.RefreshAsync(token);
    
        return result;
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
    /// <param name="guid">Guid для отправки его в ссылке.</param>
    /// <param name="userId">Id пользователя.</param>
    public async Task AddRestoreUserDataCacheAsync(Guid guid, long userId)
    {
        await _redisCache.SetStringAsync(userId.ToString(), ProtoBufExtensions.Serialize(guid),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
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

        var redisResult = ProtoBufExtensions.Deserialize<Guid>(result);
        
        if (redisResult == Guid.Empty)
        {
            throw new InvalidOperationException(
                "Не удалось получить код из кэша для проверки восстановления пароля пользователя.");
        }

        return true;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}