using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Logs.Abstractions;

/// <summary>
/// Базовый класс для логирования.
/// </summary>
public abstract class BaseLogService
{
    protected readonly PgContext PgContext;
    
    protected BaseLogService(PgContext pgContext)
    {
        PgContext = pgContext;
    }

    /// <summary>
    /// Метод пишет логи ошибок в базу.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    /// <param name="account">Аккаунт пользователя, под которым произошло исключение.</param>
    /// <param name="logLevel">Уровень исключения.</param>
    // public async Task LogInfoAsync(Exception ex, string account, LogLevelEnum logLevel)
    // {
    //     return;
    // }
}