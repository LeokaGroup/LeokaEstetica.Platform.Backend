namespace LeokaEstetica.Platform.Notifications.Abstractions;

/// <summary>
/// Абстракция сервиса уведомлений.
/// </summary>
public interface INotificationsService
{
    /// <summary>
    /// Метод отправляет уведомление об успешном сохранении.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    Task SendNotifySuccessSaveAsync(string title, string notifyText, string notificationLevel, string userCode);

    /// <summary>
    /// Метод сохраняет ConnectionId подключения SignalR в кэш.
    /// </summary>
    /// <param name="connectionId">Id подключения, который создает SignalR.</param>
    /// <param name="userCode">Код пользователя.</param>
    // Task SaveConnectionIdCacheAsync(string connectionId, string userCode);

    /// <summary>
    /// Метод отправляет уведомление с предупреждением о пустом списке навыков пользователя. Пользователь значит не выбрал навыки.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    Task SendNotificationWarningSaveUserSkillsAsync(string title, string notifyText, string notificationLevel, string userCode);
    
    /// <summary>
    /// Метод отправляет уведомление с предупреждением о пустом списке целей пользователя. Пользователь значит не выбрал навыки.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    Task SendNotificationWarningSaveUserIntentsAsync(string title, string notifyText, string notificationLevel, string userCode);
    
    /// <summary>
    /// Метод отправляет уведомление об успешном создании проекта пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    Task SendNotificationSuccessCreatedUserProjectAsync(string title, string notifyText, string notificationLevel, string userCode);
    
    /// <summary>
    /// Метод отправляет уведомление об ошибке при создании проекта пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    Task SendNotificationErrorCreatedUserProjectAsync(string title, string notifyText, string notificationLevel, string userCode);
    
    /// <summary>
    /// Метод отправляет уведомление о дубликате проекта пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    Task SendNotificationWarningDublicateUserProjectAsync(string title, string notifyText, string notificationLevel, string userCode);

    /// <summary>
    /// Метод отправляет уведомление об успешном создании вакансии.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    Task SendNotificationSuccessCreatedUserVacancyAsync(string title, string notifyText, string notificationLevel,
        string userCode);
}