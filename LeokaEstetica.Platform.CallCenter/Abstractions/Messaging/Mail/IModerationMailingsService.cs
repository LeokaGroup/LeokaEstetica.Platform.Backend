namespace LeokaEstetica.Platform.CallCenter.Abstractions.Messaging.Mail;

/// <summary>
/// Абстракция сервиса уведомлений почты модерации.
/// </summary>
public interface IModerationMailingsService
{
    /// <summary>
    /// Метод отправляет уведомление на почту владельца проекта о одобрении проекта модератором.
    /// </summary>
    /// <param name="mailTo">Почта владельца проекта.</param>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    Task SendNotificationApproveProjectAsync(string mailTo, string projectName, long projectId);
    
    /// <summary>
    /// Метод отправляет уведомление на почту владельца проекта о одобрении проекта модератором.
    /// </summary>
    /// <param name="mailTo">Почта владельца проекта.</param>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    Task SendNotificationRejectProjectAsync(string mailTo, string projectName, long projectId);
}