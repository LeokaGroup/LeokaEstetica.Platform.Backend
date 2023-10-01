namespace LeokaEstetica.Platform.Notifications.Abstractions;

/// <summary>
/// Абстракция сервиса уведомлений комментариев.
/// </summary>
public interface ICommentNotificationsService
{
    /// <summary>
    /// Метод отправляет уведомление о том что комментарий к проекту не может быть пустым.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    Task SendNotificationCommentProjectIsNotEmptyAsync(string title, string notifyText,
        string notificationLevel, string token);
    
    /// <summary>
    /// Метод отправляет уведомление успешной записи комментария проекта.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    Task SendNotificationSuccessCreatedCommentProjectAsync(string title, string notifyText,
        string notificationLevel, string token);
}
