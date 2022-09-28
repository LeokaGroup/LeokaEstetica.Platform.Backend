using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Logs.Abstractions;

/// <summary>
/// Абстракция сервиса логирования.
/// </summary>
public interface ILogService
{
    /// <summary>
    /// Метод пишет логи ошибок в базу.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    /// <param name="account">Аккаунт пользователя, под которым произошло исключение.</param>
    /// <param name="logLevel">Уровень исключения.</param>
    Task LogInfoAsync(Exception ex, string account, LogLevelEnum logLevel);
}