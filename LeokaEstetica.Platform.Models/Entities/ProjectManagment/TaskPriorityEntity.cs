namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

// Класс сопоставляется с таблицей приоритетов задач.
public class TaskPriorityEntity
{
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