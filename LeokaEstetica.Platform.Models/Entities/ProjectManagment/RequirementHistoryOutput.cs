namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс выходной модели для историй, требований. Этот класс относится к типу задачи "История".
/// </summary>
public class RequirementUserStoryOutput
{
    /// <summary>
    /// PK.
    /// Id истории.
    /// </summary>
    public long StoryId { get; set; }

    /// <summary>
    /// Название истории.
    /// </summary>
    public string StoryName { get; set; }

    /// <summary>
    /// Описание истории.
    /// </summary>
    public string StoryDescription { get; set; }

    /// <summary>
    /// Пользователь, который создал историю.
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// Дата создания истории.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата обновления истории.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Пользователь, который обновил историю.
    /// </summary>
    public long? UpdatedBy { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Дата начала истории.
    /// </summary>
    public DateTime? DateStart { get; set; }
    
    /// <summary>
    /// Дата окончания истории.
    /// </summary>
    public DateTime? DateEnd { get; set; }

    /// <summary>
    /// Приоритет истории.
    /// </summary>
    public int PriorityId { get; set; }
    
    /// <summary>
    /// Id резолюции.
    /// </summary>
    public int? ResolutionId { get; set; }

    /// <summary>
    /// Список Id тегов истории.
    /// </summary>
    public int[] TagIds { get; set; }
    
    /// <summary>
    /// Id статуса истории.
    /// </summary>
    public long StoryStatusId { get; set; }

    /// <summary>
    /// Id истории в рамках проекта.
    /// </summary>
    public long UserStoryTaskId { get; set; }
}