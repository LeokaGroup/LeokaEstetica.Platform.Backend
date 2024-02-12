using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели удаления связи между задачами.
/// </summary>
public class RemoveTaskLinkInput
{
    /// <summary>
    /// Тип связи задачи.
    /// </summary>
    public LinkTypeEnum LinkType { get; set; }

    /// <summary>
    /// Id задачи, с которой разрывается связь (задача в рамках проекта).
    /// </summary>
    public long RemovedLinkId { get; set; }

    /// <summary>
    /// Id текущей задачи (задача в рамках проекта).
    /// </summary>
    public long CurrentTaskId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }
}