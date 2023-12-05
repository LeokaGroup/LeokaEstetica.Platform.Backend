namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сопоставляется с таблицей истории действий над задачей.
/// </summary>
public class TaskHistoryEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long HistoryId { get; set; }

    /// <summary>
    /// Id действия, которое совершили над задачей.
    /// </summary>
    public int ActionId { get; set; }
    
    /// <summary>
    /// Дата создания действия.
    /// </summary>
    public DateTime Created { get; set; }
    
    /// <summary>
    /// Дата обновления комментария.
    /// </summary>
    public DateTime Updated { get; set; }

    /// <summary>
    /// Id задачи.
    /// </summary>
    public long TaskId { get; set; }

    /// <summary>
    /// FK на задачу.
    /// </summary>
    public UserTaskEntity UserTask { get; set; }

    /// <summary>
    /// FK на действие.
    /// </summary>
    public HistoryActionEntity HistoryAction { get; set; }
}