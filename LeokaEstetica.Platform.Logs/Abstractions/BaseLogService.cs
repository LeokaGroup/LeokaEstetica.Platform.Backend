using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Models.Entities.Log;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Logs.Abstractions;

/// <summary>
/// Базовый класс для логирования.
/// </summary>
public abstract class BaseLogService<T> where T : class
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
    public virtual async Task LogInfoAsync(Exception ex, string account, LogLevelEnum logLevel)
    {
        try
        {
            await _pgContext.LogInfos.AddAsync(new LogInfoEntity
            {
                ExceptionMessage = ex.Message,
                DateCreated = DateTime.UtcNow,
                StackTrace = ex.StackTrace,
                Account = account,
                LogLevel = logLevel,
                LogKey = Guid.NewGuid()
            });
            await _pgContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}