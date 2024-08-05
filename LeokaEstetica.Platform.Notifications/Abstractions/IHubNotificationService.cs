using LeokaEstetica.Platform.Redis.Enums;

namespace LeokaEstetica.Platform.Notifications.Abstractions;

/// <summary>
/// Абстракция сервиса уведомлений хабов.
/// </summary>
public interface IHubNotificationService
{
    /// <summary>
    /// Метод отправляет уведомлений через указанный хаб.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Тип уведомления.</param>
    /// <param name="function">Название функции хаба (нужно для фронта).</param>
    /// <param name="userCode">Код пользователя.</param>
    /// <param name="module">Тип модуля приложения.</param>
    Task SendNotificationAsync(string title, string notifyText, string notificationLevel, string function,
        Guid userCode, UserConnectionModuleEnum module);
    
    /// <summary>
    /// Метод отправляет результат классификации на фронт в чат.
    /// </summary>
    /// <param name="message">Сообщение для чата на фронт.</param>
    /// <param name="connectionId">Id подключения сокетов.</param>
    /// <param name="dialogId">Id диалога.</param>
    Task SendClassificationNetworkMessageResultAsync(string message, string connectionId, long dialogId);
}