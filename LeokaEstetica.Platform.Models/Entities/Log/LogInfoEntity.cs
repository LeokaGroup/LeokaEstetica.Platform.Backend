using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Models.Entities.Log;

/// <summary>
/// Класс сопоставляется с таблицей dbo.Logs.
/// </summary>
public class LogInfoEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long LogId { get; set; }
    
    /// <summary>
    /// Текст сообщения.
    /// </summary>
    public string ExceptionMessage { get; set; }

    /// <summary>
    /// Уровень исключения.
    /// </summary>
    public LogLevelEnum LogLevel { get; set; }

    /// <summary>
    /// Трассировка стека исключения.
    /// </summary>
    public string StackTrace { get; set; }

    /// <summary>
    /// Дата лога.
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Аккаунт пользователя.
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// Ключ лога, нужен для быстрого поиска лога.
    /// </summary>
    public Guid LogKey { get; set; }
}