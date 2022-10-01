namespace LeokaEstetica.Platform.Logs.Abstractions;

/// <summary>
/// Абстракция сервиса логирования.
/// </summary>
public interface ILogService
{
    /// <summary>
    /// Метод пишет логи ошибок в базу с уровнем Error.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    Task LogErrorAsync(Exception ex);
    
    /// <summary>
    /// Метод пишет логи ошибок в базу с уровнем Info.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    Task LogInfoAsync(Exception ex);
    
    /// <summary>
    /// Метод пишет логи ошибок в базу с уровнем Critical.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    Task LogCriticalAsync(Exception ex);
    
    /// <summary>
    /// Метод пишет логи ошибок в базу с уровнем Warning.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    Task LogWarningAsync(Exception ex);
}