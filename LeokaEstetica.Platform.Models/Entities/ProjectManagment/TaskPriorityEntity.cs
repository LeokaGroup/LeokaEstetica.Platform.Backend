namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

// Класс сопоставляется с таблицей приоритетов задач.
public class TaskPriorityEntity
{
    public TaskPriorityEntity(string priorityName, string prioritySysName)
    {
        PriorityName = priorityName;
        PrioritySysName = prioritySysName;
    }
    /// <summary>
    /// PK.
    /// </summary>
    public int PriorityId { get; set; }

    /// <summary>
    /// Название приоритета.
    /// </summary>
    public string PriorityName { get; set; }
    
    /// <summary>
    /// Системное название приоритета.
    /// </summary>
    public string PrioritySysName { get; set; }

    /// <summary>
    /// Порядковый номер.
    /// </summary>
    public int Position { get; set; }
}