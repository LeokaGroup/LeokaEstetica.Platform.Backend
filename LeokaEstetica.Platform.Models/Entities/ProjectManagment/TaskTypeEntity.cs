namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сопоставляется с таблицей типов задач.
/// </summary>
public class TaskTypeEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int TypeId { get; set; }

    /// <summary>
    /// Название типа.
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    /// Системное название типа.
    /// </summary>
    public string TypeSysName { get; set; }
    
    /// <summary>
    /// Порядковый номер.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// FK на задачу.
    /// </summary>
    public UserTaskEntity UserTask { get; set; }
}