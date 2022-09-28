namespace LeokaEstetica.Platform.Models.Entities.Logs;

/// <summary>
/// Класс сопоставляется с таблицей Logs.LogInfo.
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
    /// Дата создания лога.
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Аккаунт пользователя, под которым записали лог.
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// Трассировка стека.
    /// </summary>
    public string StackTrace { get; set; }

    /// <summary>
    /// Ключ лога. Нужен для поиска лога.
    /// </summary>
    public Guid LogKey { get; set; }

    /// <summary>
    /// Уровень логирования.
    /// </summary>
    public string LogLevel { get; set; }

    /// <summary>
    /// Исключение.
    /// </summary>
    public string InnerException { get; set; }
}