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
    public async Task LogErrorAsync(Exception ex)
    {
        await _pgContext.LogInfos.AddAsync(new LogInfoEntity
        {
            ExceptionMessage = ex.Message,
            DateCreated = DateTime.UtcNow,
            StackTrace = ex.StackTrace,
            LogLevel = LogLevelEnum.Error.ToString(),
            InnerException = ex.InnerException?.ToString(),
            LogKey = Guid.NewGuid()
        });
        await _pgContext.SaveChangesAsync();
    }

    public async Task LogInfoAsync(Exception ex)
    {
        await _pgContext.LogInfos.AddAsync(new LogInfoEntity
        {
            ExceptionMessage = ex.Message,
            DateCreated = DateTime.UtcNow,
            StackTrace = ex.StackTrace,
            LogLevel = LogLevelEnum.Info.ToString(),
            InnerException = ex.InnerException?.ToString(),
            LogKey = Guid.NewGuid()
        });
        await _pgContext.SaveChangesAsync();
    }

    public async Task LogCriticalAsync(Exception ex)
    {
        await _pgContext.LogInfos.AddAsync(new LogInfoEntity
        {
            ExceptionMessage = ex.Message,
            DateCreated = DateTime.UtcNow,
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
    public async Task LogWarningAsync(Exception ex)
    {
        await _pgContext.LogInfos.AddAsync(new LogInfoEntity
        {
            ExceptionMessage = ex.Message,
            DateCreated = DateTime.UtcNow,
            StackTrace = ex.StackTrace,
            LogLevel = LogLevelEnum.Warning.ToString(),
            InnerException = ex.InnerException?.ToString(),
            LogKey = Guid.NewGuid()
        });
        await _pgContext.SaveChangesAsync();
    }
}