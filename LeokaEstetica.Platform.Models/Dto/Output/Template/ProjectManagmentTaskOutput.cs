namespace LeokaEstetica.Platform.Models.Dto.Output.Template;

/// <summary>
/// Класс выходной модели задач проекта.
/// </summary>
public class ProjectManagmentTaskOutput
{
    /// <summary>
    /// Id задачи в системе.
    /// </summary>
    public long TaskId { get; set; }

    /// <summary>
    /// Название статуса задачи.
    /// </summary>
    public string TaskStatusName { get; set; }

    /// <summary>
    /// TODO: В будущем будет изменен на объект, содержащий фото и тд.
    /// ФИО автора задачи.
    /// </summary>
    public string AuthorName { get; set; }

    /// <summary>
    /// TODO: В будущем будет изменен на объект, содержащий фото и тд.
    /// ФИО наблюдателей задачи.
    /// </summary>
    public List<string> WatcherNames { get; set; }

    /// <summary>
    /// Название задачи.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Описание задачи.
    /// </summary>
    public string Details { get; set; }

    /// <summary>
    /// Дата создания задачи.
    /// </summary>
    public DateTime Created { get; set; }
    
    /// <summary>
    /// Дата обновление задачи.
    /// </summary>
    public DateTime Updated { get; set; }

    /// <summary>
    /// Id задачи в рамках проекта.
    /// </summary>
    public long ProjectTaskId { get; set; }

    /// <summary>
    /// Название резолюции (если указана).
    /// </summary>
    public string ResolutionName { get; set; }

    /// <summary>
    /// TODO: В будущем будет изменен на объект, содержащий цвета и тд.
    /// Названия меток (тегов) задачи.
    /// </summary>
    public List<string> TagNames { get; set; }

    /// <summary>
    /// Название типа задачи.
    /// </summary>
    public string TaskTypeName { get; set; }

    /// <summary>
    /// TODO: В будущем будет изменен на объект, содержащий фото и тд.
    /// ФИО исполнителя задачи.
    /// </summary>
    public long ExecutorName { get; set; }
}