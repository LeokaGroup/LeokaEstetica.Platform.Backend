namespace LeokaEstetica.Platform.Models.Dto.Output.Notification;

/// <summary>
/// Класс выходной модели приглашений в проект.
/// </summary>
public class ProjectInviteOutput
{
    /// <summary>
    /// PK.
    /// </summary>
    public long NotificationId { get; set; }

    /// <summary>
    /// Email пользователя, которому отправлено приглашение.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Id пользователя, которому отправлено приглашение в проект.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Дата отправки приглашения.
    /// </summary>
    public string? CreatedAt { get; set; }
}