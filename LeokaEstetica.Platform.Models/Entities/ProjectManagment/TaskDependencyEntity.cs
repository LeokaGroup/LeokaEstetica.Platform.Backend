namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сопоставляется с таблицей зависимостей задач.
/// </summary>
public class TaskDependencyEntity
{
    public TaskDependencyEntity(string dependencySysType, string dependencyTypeName)
    {
        DependencySysType = dependencySysType;
        DependencyTypeName = dependencyTypeName;    
    }
    /// <summary>
    /// PK.
    /// </summary>
    public long DependencyId { get; set; }
    
    /// <summary>
    /// Id задачи.
    /// </summary>
    public long TaskId { get; set; }

    /// <summary>
    /// Задачи пользователя.
    /// </summary>
    public ProjectTaskEntity ProjectTask { get; set; }

    /// <summary>
    /// Системное название типа зависимости.
    /// </summary>
    public string DependencySysType { get; set; }

    /// <summary>
    /// Название типа зависимости.
    /// (например, Блокирует/блокируется, Клонирует/клонируется, Дублирует/дублируется, Связано с).
    /// </summary>
    public string DependencyTypeName { get; set; }

    /// <summary>
    /// Порядковый номер.
    /// </summary>
    public int Position { get; set; }
}