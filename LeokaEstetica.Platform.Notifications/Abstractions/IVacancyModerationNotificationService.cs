namespace LeokaEstetica.Platform.Notifications.Abstractions;

/// <summary>
/// Абстракция сервиса уведомлений модерации вакансий.
/// </summary>
public interface IVacancyModerationNotificationService
{
    /// <summary>
    /// Метод отправляет уведомление об успешном сохранении замечаний вакансии.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    Task SendNotificationSuccessCreateVacancyRemarksAsync(string title, string notifyText, string notificationLevel,
        string token);
}