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
    /// <param name="errorText">Дополнительный текст ошибки.
    /// Добавлять по необходимости перед текстом исключения.</param>
    Task LogErrorAsync(Exception ex, string errorText = null);
    
    /// <summary>
    /// Метод пишет логи ошибок в базу с уровнем Info.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    /// Добавлять по необходимости перед текстом исключения.</param>
    Task LogInfoAsync(Exception ex, string errorText = null);
    
    /// <summary>
    /// Метод пишет логи ошибок в базу с уровнем Critical.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    /// Добавлять по необходимости перед текстом исключения.</param>
    Task LogCriticalAsync(Exception ex, string errorText = null);
    
    /// <summary>
    /// Метод пишет логи ошибок в базу с уровнем Warning.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    /// Добавлять по необходимости перед текстом исключения.</param>
    Task LogWarningAsync(Exception ex, string errorText = null);
    
    /// <summary>
    /// Метод пишет логи ошибок в базу с уровнем Error.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    /// Добавлять по необходимости перед текстом исключения.</param>
    void LogError(Exception ex, string errorText = null);
    
    /// <summary>
    /// Метод пишет логи ошибок в базу с уровнем Info.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    /// Добавлять по необходимости перед текстом исключения.</param>
    void LogInfo(Exception ex, string errorText = null);
    
    /// <summary>
    /// Метод пишет логи ошибок в базу с уровнем Critical.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    /// Добавлять по необходимости перед текстом исключения.</param>
    void LogCritical(Exception ex, string errorText = null);
    
    /// <summary>
    /// Метод пишет логи ошибок в базу с уровнем Warning.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    /// Добавлять по необходимости перед текстом исключения.</param>
    void LogWarning(Exception ex, string errorText = null);
}