using Microsoft.AspNetCore.Mvc;

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
    /// Id статуса задачи.
    /// </summary>
    public int TaskStatusId { get; set; }

    /// <summary>
    /// Id автора задачи.
    /// </summary>
    public long AuthorId { get; set; }

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
    /// Id наблюдателей задачи.
    /// </summary>
    public List<long> WatcherIds { get; set; }

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
    public string Created { get; set; }
    
    /// <summary>
    /// Дата обновление задачи.
    /// </summary>
    public string Updated { get; set; }

    /// <summary>
    /// Id задачи в рамках проекта.
    /// </summary>
    public long ProjectTaskId { get; set; }

    /// <summary>
    /// Название резолюции (если указана).
    /// </summary>
    public string ResolutionName { get; set; }

    /// <summary>
    /// Id резолюции (если указана).
    /// </summary>
    public int ResolutionId { get; set; }

    /// <summary>
    /// TODO: В будущем будет изменен на объект, содержащий цвета и тд.
    /// Названия меток (тегов) задачи.
    /// </summary>
    public List<string> TagNames { get; set; }

    /// <summary>
    /// Id тегов (меток).
    /// </summary>
    public List<int> TagIds { get; set; }

    /// <summary>
    /// Название типа задачи.
    /// </summary>
    public string TaskTypeName { get; set; }

    /// <summary>
    /// Id типа задачи.
    /// </summary>
    public int TaskTypeId { get; set; }

    /// <summary>
    /// Данные исполнителя задачи.
    /// </summary>
    public Executor Executor { get; set; }

    /// <summary>
    /// Id исполнителя задачи.
    /// </summary>
    public long ExecutorId { get; set; }
    
    /// <summary>
    /// Id приоритета задачи (если указана).
    /// </summary>
    public int PriorityId { get; set; }

    /// <summary>
    /// Название приоритета задачи.
    /// </summary>
    public string PriorityName { get; set; }

    /// <summary>
    /// Префикс номера задачи.
    /// </summary>
    public string TaskIdPrefix { get; set; }

    /// <summary>
    /// Id задачи в рамках проекта вместе с префиксом.
    /// </summary>
    public string FullProjectTaskId => string.Concat(TaskIdPrefix + "-", ProjectTaskId);
    
    /// <summary>
    /// Id задачи вместе с префиксом.
    /// </summary>
    public string FullTaskId => string.Concat(TaskIdPrefix + "-", TaskId);
}

/// <summary>
/// Класс исполнителя задачи.
/// </summary>
public class Executor
{
    /// <summary>
    /// ФИО исполнителя задачи.
    /// </summary>
    public string ExecutorName { get; set; }

    /// <summary>
    /// Файл аватара исполнителя.
    /// </summary>
    public FileContentResult Avatar { get; set; }
}