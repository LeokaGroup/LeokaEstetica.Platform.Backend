using LeokaEstetica.Platform.Models.Dto.Output.Notification;

namespace LeokaEstetica.Platform.Notifications.Abstractions;

/// <summary>
/// Абстракция сервиса уведомлений проектов.
/// </summary>
public interface IProjectNotificationsService
{
    /// <summary>
    /// Метод отправляет уведомление об успешном создании проекта пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    Task SendNotificationSuccessCreatedUserProjectAsync(string title, string notifyText, string notificationLevel);

    /// <summary>
    /// Метод отправляет уведомление об ошибке при создании проекта пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    Task SendNotificationErrorCreatedUserProjectAsync(string title, string notifyText, string notificationLevel);

    /// <summary>
    /// Метод отправляет уведомление о дубликате проекта пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    Task SendNotificationWarningDublicateUserProjectAsync(string title, string notifyText, string notificationLevel);

    /// <summary>
    /// Метод отправляет уведомление об успехе при изменении проекта пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    Task SendNotificationSuccessUpdatedUserProjectAsync(string title, string notifyText, string notificationLevel);

    /// <summary>
    /// Метод отправляет уведомление об ошибке при изменении проекта пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    Task SendNotificationErrorUpdatedUserProjectAsync(string title, string notifyText, string notificationLevel);

    /// <summary>
    /// Метод отправляет уведомление об успешной привязке вакансии к проекту.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    Task SendNotificationSuccessAttachProjectVacancyAsync(string title, string notifyText, string notificationLevel);

    /// <summary>
    /// Метод отправляет уведомление об дубликате при привязке вакансии к проекту.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    Task SendNotificationErrorDublicateAttachProjectVacancyAsync(string title, string notifyText,
        string notificationLevel);

    /// <summary>
    /// Метод отправляет уведомление об отклике на проект.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    Task SendNotificationSuccessProjectResponseAsync(string title, string notifyText, string notificationLevel);
    
    /// <summary>
    /// Метод отправляет уведомление об ошибке при отклике на проект.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    Task SendNotificationErrorProjectResponseAsync(string title, string notifyText, string notificationLevel);

    /// <summary>
    /// Метод отправляет уведомление предупреждения об отклике на проект.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    Task SendNotificationWarningProjectResponseAsync(string title, string notifyText, string notificationLevel);

    /// <summary>
    /// Метод отправляет уведомление предупреждения о не найденных пользователях по поисковому запросу.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    /// <param name="searchText">Поисковый запрос.</param>
    Task SendNotificationWarningSearchProjectTeamMemberAsync(string title, string notifyText, string notificationLevel);

    /// <summary>
    /// Метод отправляет уведомление предупреждения об ошибке при добавлении пользователей в команду проекта.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    /// <param name="searchText">Поисковый запрос.</param>
    Task SendNotificationWarningInviteProjectTeamMembersAsync(string title, string notifyText,
        string notificationLevel);

    /// <summary>
    /// Метод отправляет уведомление ошибки об ошибке при добавлении пользователей в команду проекта.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    /// <param name="searchText">Поисковый запрос.</param>
    Task SendNotificationErrorInviteProjectTeamMembersAsync(string title, string notifyText, string notificationLevel);
    
    /// <summary>
    /// Метод отправляет уведомление об предупреждении лимите проектов по тарифу.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    Task SendNotificationWarningLimitFareRuleProjectsAsync(string title, string notifyText, string notificationLevel);
    
    /// <summary>
    /// Метод отправляет уведомление об ошибке при удалении вакансии проекта.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    Task SendNotificationErrorDeleteProjectVacancyAsync(string title, string notifyText, string notificationLevel);
    
    /// <summary>
    /// Метод отправляет уведомление об успехе при удалении вакансии проекта.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    Task SendNotificationSuccessDeleteProjectVacancyAsync(string title, string notifyText, string notificationLevel);
    
    /// <summary>
    /// Метод отправляет уведомление об ошибке при удалении проекта.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    Task SendNotificationErrorDeleteProjectAsync(string title, string notifyText, string notificationLevel);
    
    /// <summary>
    /// Метод отправляет уведомление об успехе при удалении проекта.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    Task SendNotificationSuccessDeleteProjectAsync(string title, string notifyText, string notificationLevel);

    /// <summary>
    /// Метод получает список уведомлений в проекты пользователя.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список уведомлений.</returns>
    Task<NotificationResultOutput> GetUserProjectsNotificationsAsync(string account);
}