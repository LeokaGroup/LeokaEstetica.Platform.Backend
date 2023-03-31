namespace LeokaEstetica.Platform.Messaging.Abstractions.Mail;

/// <summary>
/// Абстракция сервиса работы с сообщениями.
/// </summary>
public interface IMailingsService
{
    /// <summary>
    /// Метод отправит подтверждение на почту.
    /// </summary>
    /// <param name="mailTo">Email кому отправить.</param>
    /// <param name="confirmEmailCode">Код подтверждения почты.</param>
    Task SendConfirmEmailAsync(string mailTo, Guid confirmEmailCode);

    /// <summary>
    /// Метод отправляет уведомление на почту владельца проекта о создании проекта.
    /// Указывается вакансия, если она заполнена.
    /// </summary>
    /// <param name="mailTo">Почта владельца проекта.</param>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="publicId">Код гуида проекта для публичного отображения в роуте.</param>
    Task SendNotificationCreatedProjectAsync(string mailTo, string projectName,
        long projectId);

    /// <summary>
    /// Метод отправляет уведомление на почту владельца проекта о удалении проекта и всего связанного с ним.
    /// </summary>
    /// <param name="mailTo">Почта владельца проекта.</param>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="vacanciesNames">Список названий вакансий, которые были также удалены.</param>
    Task SendNotificationDeleteProjectAsync(string mailTo, string projectName, List<string> vacanciesNames);
    
    /// <summary>
    /// Метод отправляет уведомление на почту. владельца вакансии.
    /// </summary>
    /// <param name="mailTo">Почта владельца проекта.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    Task SendNotificationCreateVacancyAsync(string mailTo, string vacancyName, long vacancyId);
}