namespace LeokaEstetica.Platform.Models.Dto.Output.Notification;

/// <summary>
/// Класс выходной модели уведомлений.
/// </summary>
public class NotificationOutput
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
    /// Текст уведомления.
    /// </summary>
    public string NotificationText { get; set; }

    /// <summary>
    /// Признак, надо ли показывать уведомление в местах повышенных индикаторов (повышенные индикаторы -
    /// это места быстрых уведомлений в модалках и тд. Т.е. показали пользователю, пока он не нажал скрыть).
    /// IsShow может скрывать от мест повышенных индикаторов, но не влияет на отображение уведомлений в общих местах системы.
    /// </summary>
    public bool IsShow { get; set; }

    /// <summary>
    /// Признак отображения кнопки принятия инвайта в проект.
    /// </summary>
    public bool IsAcceptButton { get; set; }
    
    /// <summary>
    /// Признак отображения кнопки отклонения инвайта в проект.
    /// </summary>
    public bool IsRejectButton { get; set; }

    /// <summary>
    /// Признак необходимости подтверждения уведомления.
    /// </summary>
    public bool IsNeedAccepted { get; set; }
    
    /// <summary>
    /// Признак владельца приглашения.
    /// Если true, то инициатором приглашения был владелец, при false другой пользователь.
    /// </summary>
    public bool IsOwner { get; set; }
    
    /// <summary>
    /// Признак отображения кнопок принятия и отклонения приглашений уведомлений.
    /// </summary>
    public bool IsVisibleNotificationsButtons { get; set; }

    /// <summary>
    /// Id пользователя, кому принадлежит уведомление.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }
}