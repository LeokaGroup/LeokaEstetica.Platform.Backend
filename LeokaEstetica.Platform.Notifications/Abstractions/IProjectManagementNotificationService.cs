namespace LeokaEstetica.Platform.Notifications.Abstractions;

/// <summary>
/// Абстракция сервиса уведомлений модуля УП - управление проектами.
/// </summary>
public interface IProjectManagementNotificationService
{
    /// <summary>
    /// Метод отправляет уведомление об успешном планировании спринта.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    Task SendNotifySuccessPlaningSprintAsync(string title, string notifyText, string notificationLevel, string token);
    
    /// <summary>
    /// Метод отправляет уведомление об успешном добавлении задачи в эпик.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    Task SendNotifySuccessIncludeEpicTaskAsync(string title, string notifyText, string notificationLevel,
        string token);
    
    /// <summary>
    /// Метод отправляет уведомление об ошибке при добавлении задачи в эпик.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    Task SendNotifyErrorIncludeEpicTaskAsync(string title, string notifyText, string notificationLevel,
        string token);
    
    /// <summary>
    /// Метод отправляет уведомление о предупреждении дубликата задачи в проекте.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    Task SendNotifyWarningDublicateProjectTaskAsync(string title, string notifyText, string notificationLevel,
        string token);

    /// <summary>
    /// Метод отправляет результат классификации на фронт в чат.
    /// </summary>
    /// <param name="message">Сообщение для чата на фронт.</param>
    /// <param name="connectionId">Id подключения сокетов.</param>
    /// <param name="dialogId">Id диалога.</param>
    Task SendClassificationNetworkMessageResultAsync(string message, string connectionId, long dialogId);
    
    /// <summary>
    /// Метод отправляет уведомление об успешном обновлении ролей.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    Task SendNotifySuccessUpdateRolesAsync(string title, string notifyText, string notificationLevel, string token);
    
    /// <summary>
    /// Метод отправляет уведомление о предупреждении невозможности изменения статуса эпика на недопустимый статус.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    Task SendNotifyWarningChangeEpicStatusAsync(string title, string notifyText, string notificationLevel,
        string token);
    
    /// <summary>
    /// Метод отправляет уведомление о предупреждении невозможности изменения статуса истории на недопустимый статус.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    Task SendNotifyWarningChangeStoryStatusAsync(string title, string notifyText, string notificationLevel,
        string token);
}