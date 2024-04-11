using System.Globalization;
using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Base.Factors;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Models.Entities.Configs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Database.Repositories.Config;

/// <summary>
/// Класс реализует методы репозитория глобал конфига.
/// </summary>
internal sealed class GlobalConfigRepository : IGlobalConfigRepository
{
    private readonly PgContext _pgContext;
    private readonly ILogger<GlobalConfigRepository> _logger;
    private readonly IConfiguration _configuration;

    public GlobalConfigRepository(PgContext pgContext,
        ILogger<GlobalConfigRepository> logger,
        IConfiguration configuration)
    {
        _pgContext = pgContext;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Метод получает значение по ключу.
    /// </summary>
    /// <param name="key">Ключ для получения значения.</param>
    /// <typeparam name="T">Значение, тип которого определится дженериками.</typeparam>
    /// <returns>Значение из конфига.</returns>
    public async Task<T> GetValueByKeyAsync<T>(string key)
    {
        var type = typeof(T);
        GlobalConfigEntity config;

        try
        {
            config = await _pgContext.GlobalConfig
                .Where(gc => gc.ParamKey.Equals(key))
                .FirstOrDefaultAsync();
        }
        
        // TODO: При dispose PgContext пересоздаем датаконтекст и пробуем снова.
        catch (ObjectDisposedException _)
        {
            var pgContext = CreateNewPgContextFactory.CreateNewPgContext(_configuration);
            config = await pgContext.GlobalConfig
                .Where(gc => gc.ParamKey.Equals(key))
                .FirstOrDefaultAsync();
        }

        if (config == null)
        {
            // Если мы не бросим ошибку тут, то в логике получим timeSpan 00:00:00, вместо NULL, как указали в бд
            if (type == typeof(TimeSpan))
            {
                throw new InvalidOperationException("Нельзя nullable TimeSpan");
            }

            return default;
        }

        if (string.IsNullOrEmpty(config.ParamValue))
        {
            // Если мы не бросим ошибку тут, то в логике получим timeSpan 00:00:00, вместо NULL, как указали в бд
            if (type == typeof(TimeSpan))
            {
                throw new InvalidOperationException("Нельзя nullable TimeSpan");
            }

            return default;
        }

        var result = default(T);

        try
        {
            // Распаковываем nullable тип, чтобы произвести корректную проверку типов.
            var nullableType = Nullable.GetUnderlyingType(type);

            if (nullableType != null)
            {
                type = nullableType;
            }

            if (type.IsArray)
            {
                result = config.ParamValue.GetStringAsArray<T>();
            }

            else if (type == typeof(TimeSpan))
            {
                result = (T)(object)TimeSpan.Parse(config.ParamValue);
            }

            else if (type.BaseType == typeof(Enum))
            {
                result = (T)Enum.Parse(type, config.ParamValue);
            }

            else if (type == typeof(double))
            {
                result = (T)Convert.ChangeType(config.ParamValue.Replace(",", "."),
                    typeof(T), CultureInfo.InvariantCulture);
            }

            else if (type == typeof(float))
            {
                result = (T)Convert.ChangeType(config.ParamValue.Replace(",", "."),
                    typeof(T), CultureInfo.InvariantCulture);
            }

            else if (type == typeof(decimal))
            {
                result = (T)Convert.ChangeType(config.ParamValue.Replace(",", "."),
                    typeof(T), CultureInfo.InvariantCulture);
            }

            else if (type == typeof(bool))
            {
                result = (T)Convert.ChangeType(config.ParamValue.ToLower(),
                    typeof(T), CultureInfo.InvariantCulture);
            }

            else
            {
                result = (T)Convert.ChangeType(config.ParamValue, typeof(T), CultureInfo.InvariantCulture);
            }
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка конвертации значения глобал конфига. " +
                                            $"Ключ = {key}; " +
                                            $"Значение = {config.ParamValue}");
        }

        return result;
    }

    /// <summary>
    /// Метод получает все данные по ключу.
    /// </summary>
    /// <param name="key">Ключ для получения значения.</param>
    /// <returns>Данные из конфига.</returns>
    public async Task<GlobalConfigEntity> GetConfigByKeyAsync(string key)
    {
        var result = await _pgContext.GlobalConfig
            .Where(gc => gc.ParamKey.Equals(key))
            .FirstOrDefaultAsync();

        return result;
    }
}