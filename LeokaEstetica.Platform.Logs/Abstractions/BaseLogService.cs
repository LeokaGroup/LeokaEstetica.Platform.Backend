using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Models.Entities.Logs;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Logs.Abstractions;

/// <summary>
/// Базовый класс для логирования.
/// </summary>
public abstract class BaseLogService
{
    private readonly PgContext _pgContext;
    
    protected BaseLogService(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод пишет логи ошибок в базу.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    /// <param name="account">Аккаунт пользователя, под которым произошло исключение.</param>
    /// <param name="logLevel">Уровень исключения.</param>
    public async Task LogInfoAsync(Exception ex, string account, LogLevelEnum logLevel)
    {
        await _pgContext.LogInfos.AddAsync(new LogInfoEntity
        {
            ExceptionMessage = ex.Message,
            DateCreated = DateTime.UtcNow,
            StackTrace = ex.StackTrace,
            Account = account,
            LogLevel = logLevel,
            InnerException = ex.InnerException?.ToString()
        });
        await _pgContext.SaveChangesAsync();
    }
}