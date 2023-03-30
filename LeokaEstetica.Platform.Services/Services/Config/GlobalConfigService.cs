using LeokaEstetica.Platform.Models.Entities.Configs;
using LeokaEstetica.Platform.Services.Abstractions.Config;

namespace LeokaEstetica.Platform.Services.Services.Config;

/// <summary>
/// Класс реализует методы сервиса глобал конфига.
/// </summary>
public class GlobalConfigService : IGlobalConfigService
{
    /// <summary>
    /// Метод получает значение по ключу.
    /// </summary>
    /// <param name="key">Ключ для получения значения.</param>
    /// <typeparam name="T">Значение, тип которого определится дженериками.</typeparam>
    /// <returns>Значение из конфига.</returns>
    public Task<T> GetValueByKeyAsync<T>(string key)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Метод получает все данные по ключу.
    /// </summary>
    /// <param name="key">Ключ для получения значения.</param>
    /// <returns>Данные из конфига.</returns>
    public GlobalConfigEntity GetConfigByKeyAsync(string key)
    {
        throw new NotImplementedException();
    }
}