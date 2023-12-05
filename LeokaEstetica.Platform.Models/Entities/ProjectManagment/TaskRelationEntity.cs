namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сопоставляется с таблицей отношений между задачами.
/// </summary>
public class TaskRelationEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long RelationId { get; set; }

    /// <summary>
    /// Тип отношения (дочка, родитель).
    /// </summary>
    public string RelationType { get; set; }

    /// <summary>
    /// Id задачи.
    /// </summary>
    public long TaskId { get; set; }

    /// <summary>
    /// Задачи пользователя.
    /// </summary>
    public UserTaskEntity UserTask { get; set; }
}