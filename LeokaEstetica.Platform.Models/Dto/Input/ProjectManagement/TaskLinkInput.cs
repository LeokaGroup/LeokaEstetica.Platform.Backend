using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели связи с задачей.
/// </summary>
public class TaskLinkInput
{
    /// <summary>
    /// Id задачи, от которой исходит связь.
    /// </summary>
    public long TaskFromLink { get; set; }

    /// <summary>
    /// Id задачи, которую связывают.
    /// </summary>
    public long TaskToLink { get; set; }

    /// <summary>
    /// Тип связи задачи.
    /// </summary>
    public LinkTypeEnum LinkType { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Id родительской задачи.
    /// Дочерней задачей при этом становится текущая задача, в которой создается связь.
    /// </summary>
    public long? ParentId { get; set; }
    
    /// <summary>
    /// Id дочерней задачи.
    /// Родительской задачей при этом становится текущая задача, в которой создается связь.
    /// </summary>
    public long? ChildId { get; set; }

    /// <summary>
    /// Id задачи, от которой зависит текущая.
    /// При этом указанная задача становится блокирующей текущую.
    /// У текущей выставляется "зависит от" указанной задачи.
    /// </summary>
    public long? DependId { get; set; }
}