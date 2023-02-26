namespace LeokaEstetica.Platform.Models.Entities.Notification;

/// <summary>
/// Класс сопоставляется с таблицей с таблицей уведомлений Notifications.Notifications.
/// </summary>
public class NotificationEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long NotificationId { get; set; }

    /// <summary>
    /// Название уведомления.
    /// </summary>
    public string NotificationName { get; set; }

    /// <summary>
    /// Системное название уведомления.
    /// </summary>
    public string NotificationSysName { get; set; }

    /// <summary>
    /// Требуется ли подтверждение для уведомления.
    /// </summary>
    public bool IsNeedAccepted { get; set; }

    /// <summary>
    /// Признак подтверждения уведомления. Зависит от IsNeedAccepted.
    /// </summary>
    public bool Approved { get; set; }

    /// <summary>
    /// Признак отклонения уведомления. Зависит от IsNeedAccepted.
    /// </summary>
    public bool Rejected { get; set; }

    /// <summary>
    /// Id объекта уведомления (например, проект). 
    /// </summary>
    public long ObjectId { get; set; }

    /// <summary>
    /// Id пользователя, который будет видеть уведомления.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Текст уведомления.
    /// </summary>
    public string NotificationText { get; set; }

    /// <summary>
    /// Дата создания уведомления.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Тип уведомления (например, приглашения).
    /// </summary>
    public string NotificationType { get; set; }

    /// <summary>
    /// Признак, надо ли показывать уведомление в местах повышенных индикаторов (повышенные индикаторы -
    /// это места быстрых уведомлений в модалках и тд. Т.е. показали пользователю, пока он не нажал скрыть).
    /// IsShow может скрывать от мест повышенных индикаторов, но не влияет на отображение уведомлений в общих местах системы.
    /// </summary>
    public bool IsShow { get; set; }
}