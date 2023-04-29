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
    
    /// <summary>
    /// Метод отправляет уведомление на почту владельца вакансии о одобрении вакансии модератором.
    /// </summary>
    /// <param name="mailTo">Почта владельца вакансии.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    Task SendNotificationApproveVacancyAsync(string mailTo, string vacancyName, long vacancyId);
    
    /// <summary>
    /// Метод отправляет уведомление на почту владельца вакансии о отклонении вакансии модератором.
    /// </summary>
    /// <param name="mailTo">Почта владельца вакансии.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    Task SendNotificationRejectVacancyAsync(string mailTo, string vacancyName, long vacancyId);

    /// <summary>
    /// Метод отправляет уведомление на почту пользователя, которого внесли в чёрный список.
    /// </summary>
    /// <param name="mailTo"></param>
    Task SendNotificationBlockUserAccountAsync(string mailTo);
}