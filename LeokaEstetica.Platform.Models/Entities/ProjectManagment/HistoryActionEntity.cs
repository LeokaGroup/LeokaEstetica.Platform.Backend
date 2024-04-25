namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сопоставляется с таблицей
/// </summary>
public class HistoryActionEntity
{
    public HistoryActionEntity(string actionName, string actionSysName)
    {
        ActionName = actionName;
        ActionSysName = actionSysName;
        TaskHistories = new HashSet<TaskHistoryEntity>();
    }

    /// <summary>
    /// PK.
    /// </summary>
    public int ActionId { get; set; }

    /// <summary>
    /// Название действия.
    /// </summary>
    public string ActionName { get; set; }

    /// <summary>
    /// Системное название действия.
    /// </summary>
    public string ActionSysName { get; set; }
    
    /// <summary>
    /// Порядковый номер.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Список историй действий.
    /// </summary>
    public IEnumerable<TaskHistoryEntity> TaskHistories { get; set; }
}