using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Entities.Logs;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Logs.Services;

/// <summary>
/// Сервис реализует методы логирования.
/// </summary>
public sealed class LogService : BaseLogService
{
    public LogService(PgContext pgContext) : base(pgContext)
    {
    }

    /// <summary>
    /// Метод пишет логи ошибок в базу.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    /// <param name="account">Аккаунт пользователя, под которым произошло исключение.</param>
    /// <param name="logLevel">Уровень исключения.</param>
    // public override async Task LogInfoAsync(Exception ex, string account, LogLevelEnum logLevel)
    // {
    //    
    // }
}