using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Entities.Logs;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Logs.Services;

/// <summary>
/// Сервис реализует методы логирования.
/// </summary>
public sealed class LogService : ILogService
{
    private readonly PgContext _pgContext;
    
    public LogService(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод пишет логи ошибок в базу.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    public async Task LogErrorAsync(Exception ex, string errorText = null)
    {
        await _pgContext.LogInfos.AddAsync(new LogInfoEntity
        {
            ExceptionMessage = errorText is not null
                ? string.Concat(errorText + " ", ex.Message)
                : ex.Message,
            DateCreated = DateTime.Now,
            StackTrace = ex.StackTrace,
            LogLevel = LogLevelEnum.Error.ToString(),
            InnerException = ex.InnerException?.ToString(),
            LogKey = Guid.NewGuid()
        });
        await _pgContext.SaveChangesAsync();
    }

    public async Task LogInfoAsync(Exception ex, string errorText = null)
    {
        await _pgContext.LogInfos.AddAsync(new LogInfoEntity
        {
            ExceptionMessage = errorText is not null
                ? string.Concat(errorText + " ", ex.Message)
                : ex.Message,
            DateCreated = DateTime.Now,
            StackTrace = ex.StackTrace,
            LogLevel = LogLevelEnum.Info.ToString(),
            InnerException = ex.InnerException?.ToString(),
            LogKey = Guid.NewGuid()
        });
        await _pgContext.SaveChangesAsync();
    }

    public async Task LogCriticalAsync(Exception ex, string errorText = null)
    {
        await _pgContext.LogInfos.AddAsync(new LogInfoEntity
        {
            ExceptionMessage = errorText is not null
                ? string.Concat(errorText + " ", ex.Message)
                : ex.Message,
            DateCreated = DateTime.Now,
            StackTrace = ex.StackTrace,
            LogLevel = LogLevelEnum.Critical.ToString(),
            InnerException = ex.InnerException?.ToString(),
            LogKey = Guid.NewGuid()
        });
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод пишет логи ошибок в базу с уровнем Warning.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    public async Task LogWarningAsync(Exception ex, string errorText = null)
    {
        await _pgContext.LogInfos.AddAsync(new LogInfoEntity
        {
            ExceptionMessage = errorText is not null
                ? string.Concat(errorText + " ", ex.Message)
                : ex.Message,
            DateCreated = DateTime.Now,
            StackTrace = ex.StackTrace,
            LogLevel = LogLevelEnum.Warning.ToString(),
            InnerException = ex.InnerException?.ToString(),
            LogKey = Guid.NewGuid()
        });
        await _pgContext.SaveChangesAsync();
    }

    public void LogError(Exception ex, string errorText = null)
    {
        _pgContext.LogInfos.Add(new LogInfoEntity
        {
            ExceptionMessage = errorText is not null
                ? string.Concat(errorText + " ", ex.Message)
                : ex.Message,
            DateCreated = DateTime.Now,
            StackTrace = ex.StackTrace,
            LogLevel = LogLevelEnum.Error.ToString(),
            InnerException = ex.InnerException?.ToString(),
            LogKey = Guid.NewGuid()
        });
        _pgContext.SaveChanges();
    }

    public void LogInfo(Exception ex, string errorText = null)
    {
        _pgContext.LogInfos.Add(new LogInfoEntity
        {
            ExceptionMessage = errorText is not null
                ? string.Concat(errorText + " ", ex.Message)
                : ex.Message,
            DateCreated = DateTime.Now,
            StackTrace = ex.StackTrace,
            LogLevel = LogLevelEnum.Info.ToString(),
            InnerException = ex.InnerException?.ToString(),
            LogKey = Guid.NewGuid()
        });
        _pgContext.SaveChanges();
    }

    public void LogCritical(Exception ex, string errorText = null)
    {
        _pgContext.LogInfos.Add(new LogInfoEntity
        {
            ExceptionMessage = errorText is not null
                ? string.Concat(errorText + " ", ex.Message)
                : ex.Message,
            DateCreated = DateTime.Now,
            StackTrace = ex.StackTrace,
            LogLevel = LogLevelEnum.Critical.ToString(),
            InnerException = ex.InnerException?.ToString(),
            LogKey = Guid.NewGuid()
        });
        _pgContext.SaveChanges();
    }

    public void LogWarning(Exception ex, string errorText = null)
    {
        _pgContext.LogInfos.Add(new LogInfoEntity
        {
            ExceptionMessage = errorText is not null
                ? string.Concat(errorText + " ", ex.Message)
                : ex.Message,
            DateCreated = DateTime.Now,
            StackTrace = ex.StackTrace,
            LogLevel = LogLevelEnum.Warning.ToString(),
            InnerException = ex.InnerException?.ToString(),
            LogKey = Guid.NewGuid()
        });
        _pgContext.SaveChanges();
    }
}