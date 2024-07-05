namespace LeokaEstetica.Platform.Models.Dto.Output.Notification;

/// <summary>
/// Класс выходной модели приглашения в проект на основе данных уведомления о приглашении.
/// </summary>
public class ProjectInviteNotificationOutput
{
    /// <summary>
    /// Id приглашения.
    /// </summary>
    public long NotificationId { get; set; }

    /// <summary>
    /// Id пользователя, который принял приглашение (это может быть либо владелец либо не владелец).
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Id вакансии, на которую был отклик (ее может не быть).
    /// </summary>
    public long? VacancyId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }
}