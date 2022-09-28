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
            LogLevel = logLevel.ToString(),
            InnerException = ex.InnerException?.ToString(),
            LogKey = Guid.NewGuid()
        });
        await _pgContext.SaveChangesAsync();
    }
}