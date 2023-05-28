using LeokaEstetica.Platform.Models.Entities.Configs;

namespace LeokaEstetica.Platform.Database.Abstractions.Config;

/// <summary>
/// Абстракция репозитория глобал конфига.
/// </summary>
public interface IGlobalConfigRepository
{
    /// <summary>
    /// Метод получает значение по ключу.
    /// </summary>
    /// <param name="key">Ключ для получения значения.</param>
    /// <typeparam name="T">Значение, тип которого определится дженериками.</typeparam>
    /// <returns>Значение из конфига.</returns>
    Task<T> GetValueByKeyAsync<T>(string key);
    
    /// <summary>
    /// Метод получает все данные по ключу.
    /// </summary>
    /// <param name="key">Ключ для получения значения.</param>
    /// <returns>Данные из конфига.</returns>
    Task<GlobalConfigEntity> GetConfigByKeyAsync(string key);
}