namespace LeokaEstetica.Platform.Base.Abstractions.Messaging.Mail;

/// <summary>
/// Абстракция для уведомлений на почту в базовом слое приложения если нужен доступ из различных модулей.
/// </summary>
public interface IMailingsService
{
    /// <summary>
    /// Метод отправляет уведомление на почту о принятии инвайта в проект.
    /// </summary>
    /// <param name="mailTo">Почта пользователя, которому отправлять уведомление.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="otherUser">Логин или почта пользователя, который оставил отклик.</param>
    /// <param name="isEmailNotificationsDisableModeEnabled">Признак уведомлений на почту.</param>
    /// <param name="apiUrl">API уведомлений почты.</param>
    Task SendNotificationApproveInviteProjectAsync(string mailTo, long projectId, string projectName,
        string vacancyName, string otherUser, bool isEmailNotificationsDisableModeEnabled, string apiUrl);
    
    /// <summary>
    /// Метод отправляет уведомление на почту о отклонении инвайта в проект.
    /// </summary>
    /// <param name="mailTo">Почта пользователя, которому отправлять уведомление.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="otherUser">Логин или почта пользователя, который оставил отклик.</param>
    /// <param name="isEmailNotificationsDisableModeEnabled">Признак уведомлений на почту.</param>
    /// <param name="apiUrl">API уведомлений почты.</param>
    Task SendNotificationRejectInviteProjectAsync(string mailTo, long projectId, string projectName,
        string vacancyName, string otherUser, bool isEmailNotificationsDisableModeEnabled, string apiUrl);
}