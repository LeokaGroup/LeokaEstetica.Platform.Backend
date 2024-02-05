namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели получения связей задачи.
/// </summary>
public class GetTaskLinkOutput
{
    /// <summary>
    /// TODO: Пока не используется. Вернуться, когда будет реализован вывод названия проекта.
    /// Код задачи. Состоит из краткого названия проекта + номер задачи в рамках проекта.
    /// </summary>
    public string TaskCode { get; set; }

    /// <summary>
    /// Название задачи.
    /// </summary>
    public string TaskName { get; set; }

    /// <summary>
    /// Название статуса задачи.
    /// </summary>
    public string TaskStatusName { get; set; }

    /// <summary>
    /// ФИО исполнителя задачи.
    /// </summary>
    public string ExecutorName { get; set; }

    /// <summary>
    /// Дата последнего обновления задачи.
    /// Например, 17 июля 2015 г. 17:04.
    /// </summary>
    public string LastUpdated { get; set; }

    /// <summary>
    /// Id задачи
    /// </summary>
    public long ProjectTaskId { get; set; }

    /// <summary>
    /// Id статуса.
    /// </summary>
    public long TaskStatusId { get; set; }

    /// <summary>
    /// Id исполнителя.
    /// </summary>
    public long ExecutorId { get; set; }

    /// <summary>
    /// Id приоритета.
    /// </summary>
    public int PriorityId { get; set; }

    /// <summary>
    /// Id задачи.
    /// </summary>
    public long TaskId { get; set; }
    
    /// <summary>
    /// Id блокируемой задачи.
    /// Задача, которая ее блокирует определяется по признаку IsBlocked.
    /// Блокируемая задача становится зависимой от блокирующей.
    /// </summary>
    public long? BlockedTaskId { get; set; }

    /// <summary>
    /// Id родителя.
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// Id ребенка.
    /// </summary>
    public long? ChildId { get; set; }
    
    /// <summary>
    /// Признак блокирующей задачи.
    /// </summary>
    public bool IsBlocked { get; set; }
}