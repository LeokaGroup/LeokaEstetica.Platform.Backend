namespace LeokaEstetica.Platform.Messaging.Abstractions.Mail;

/// <summary>
/// Абстракция сервиса работы с сообщениями почты.
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
    /// </summary>
    /// <param name="mailTo">Почта владельца проекта.</param>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    Task SendNotificationCreatedProjectAsync(string mailTo, string projectName, long projectId);

    /// <summary>
    /// Метод отправляет уведомление на почту владельца проекта о удалении проекта и всего связанного с ним.
    /// </summary>
    /// <param name="mailTo">Почта владельца проекта.</param>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="vacanciesNames">Список названий вакансий, которые были также удалены.</param>
    Task SendNotificationDeleteProjectAsync(string mailTo, string projectName, List<string> vacanciesNames);
    
    /// <summary>
    /// Метод отправляет уведомление на почту. владельца вакансии о создании новой вакансии.
    /// </summary>
    /// <param name="mailTo">Почта владельца проекта.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    Task SendNotificationCreateVacancyAsync(string mailTo, string vacancyName, long vacancyId);

    /// <summary>
    /// Метод отправляет уведомление на почту владельца вакансии об удалении вакансии.
    /// </summary>
    /// <param name="mailTo">Почта владельца проекта.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    Task SendNotificationDeleteVacancyAsync(string mailTo, string vacancyName);

    /// <summary>
    /// Метод отправляет уведомление на почту владельца проекта об отклике на его проект.
    /// </summary>
    /// <param name="mailTo">Почта владельца проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="otherUser">Логин или почта пользователя, который оставил отклик.</param>
    Task SendNotificationWriteResponseProjectAsync(string mailTo, long projectId, string projectName,
        string vacancyName, string otherUser);

    /// <summary>
    /// Метод отправляет уведомление на почту пользователя о приглашении его в проект.
    /// </summary>
    /// <param name="mailTo">Почта пользователя.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectOwnerName">Логин или почта владельца проекта,
    /// который пригласил пользователя в команду проекта.</param>
    Task SendNotificationInviteTeamProjectAsync(string mailTo, long projectId, string projectName,
        string projectOwnerName);

    /// <summary>
    /// Метод отправляет пользователю на почту предупреждении об удалении его аккаунта через 1 неделю,
    /// пока он не авторизуется.
    /// </summary>
    /// <param name="mailsTo">Список email пользователей.</param>
    Task SendNotificationDeactivateAccountAsync(List<string> mailsTo);
    
    /// <summary>
    /// Метод отправляет уведомление на почту пользователя, которого исключили из команды проекта.
    /// </summary>
    /// <param name="mailTo">Почта пользователя, которого исключили.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectName">Название проекта.</param>
    Task SendNotificationDeleteProjectTeamMemberAsync(string mailTo, long projectId, string projectName);

    /// <summary>
    /// Метод отправляет уведомление на почту пользователя, о добавлении его проекта в архив.
    /// </summary>
    /// <param name="mailTo">Почта пользователя, которого исключили.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectName">Название проекта.</param>
    Task SendNotificationAddProjectArchiveAsync(string mailTo, long projectId, string projectName);
}