using AutoMapper;
using LeokaEstetica.Platform.Models.Dto.Input.User;
using LeokaEstetica.Platform.Models.Entities.User;
using LeokaEstetica.Platform.Redis.Abstractions.User;
using LeokaEstetica.Platform.Redis.Consts;
using LeokaEstetica.Platform.Redis.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Redis.Services.User;

/// <summary>
/// Класс реализует методы сервиса кэша пользователей.
/// </summary>
public class UserRedisService : IUserRedisService
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

        var redisResult = ProtoBufExtensions.Deserialize<string>(items);
        
        if (string.IsNullOrEmpty(redisResult))
        {
            throw new InvalidOperationException(
                "Не удалось получить список аккаунтов пользователей из кэша для удаления.");
        }

        var finalResult = JsonConvert.DeserializeObject<List<UserEntity>>(redisResult);
        var result = _mapper.Map<List<UserEntity>>(finalResult);

        return result;
    }
}