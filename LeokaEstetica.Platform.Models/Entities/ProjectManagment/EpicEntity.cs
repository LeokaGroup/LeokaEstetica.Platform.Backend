namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// TODO: От сущностей уйдем, будет ренейм на EpicOutput, если такой уже нету.
/// Класс сопоставляется с таблицей эпиков.
/// </summary>
public class EpicEntity
{
    /// <summary>
    /// PK.
    /// Id эпика.
    /// </summary>
    public long EpicId { get; set; }

    /// <summary>
    /// Название эпика.
    /// </summary>
    public string EpicName { get; set; }

    /// <summary>
    /// Описание эпика.
    /// </summary>
    public string? EpicDescription { get; set; }

    /// <summary>
    /// Пользователь, который создал эпик.
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// Дата создания эпика.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата обновления эпика.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Пользователь, который обновил эпик.
    /// </summary>
    public long? UpdatedBy { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Дата начала эпика.
    /// </summary>
    public DateTime? DateStart { get; set; }
    
    /// <summary>
    /// Дата окончания эпика.
    /// </summary>
    public DateTime? DateEnd { get; set; }

    /// <summary>
    /// Приоритет задачи.
    /// </summary>
    public int PriorityId { get; set; }
    
    /// <summary>
    /// Id резолюции.
    /// </summary>
    public int? ResolutionId { get; set; }

    /// <summary>
    /// Список Id тегов задачи.
    /// </summary>
    public int[]? TagIds { get; set; }
    
    /// <summary>
    /// Id статуса задачи.
    /// </summary>
    public long TaskStatusId { get; set; }

    /// <summary>
    /// Id инициативы.
    /// </summary>
    public long? InitiativeId { get; set; }

    /// <summary>
    /// Id эпика в рамках проекта.
    /// </summary>
    public long ProjectEpicId { get; set; }
}