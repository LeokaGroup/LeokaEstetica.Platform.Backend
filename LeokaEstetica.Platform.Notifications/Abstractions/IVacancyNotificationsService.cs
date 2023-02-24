namespace LeokaEstetica.Platform.Notifications.Abstractions;

/// <summary>
/// Абстракция сервиса уведомлений вакансий.
/// </summary>
public interface IVacancyNotificationsService
{
    /// <summary>
    /// Метод отправляет уведомление об успешном создании вакансии.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    Task SendNotificationSuccessCreatedUserVacancyAsync(string title, string notifyText, string notificationLevel);
    
    /// <summary>
    /// Метод отправляет уведомление об ошибке при создании вакансии.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    Task SendNotificationErrorCreatedUserVacancyAsync(string title, string notifyText, string notificationLevel);
    
    /// <summary>
    /// Метод отправляет уведомление об предупреждении лимите вакансий по тарифу.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    Task SendNotificationWarningLimitFareRuleVacanciesAsync(string title, string notifyText, string notificationLevel);
    
    /// <summary>
    /// Метод отправляет уведомление об ошибке при удалении вакансии.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    Task SendNotificationErrorDeleteVacancyAsync(string title, string notifyText, string notificationLevel);
    
    /// <summary>
    /// Метод отправляет уведомление об успехе при удалении вакансии.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    Task SendNotificationSuccessDeleteVacancyAsync(string title, string notifyText, string notificationLevel);
}