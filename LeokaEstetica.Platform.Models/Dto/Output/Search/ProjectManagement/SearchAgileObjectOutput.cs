namespace LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;

/// <summary>
/// Класс выходной модели поиска.
/// </summary>
public class SearchAgileObjectOutput
{
    /// <summary>
    /// Название задачи.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Id задачи в рамках проекта.
    /// </summary>
    public long ProjectTaskId { get; set; }
    
    /// <summary>
    /// Префикс номера задачи.
    /// </summary>
    public string? TaskIdPrefix { get; set; }

    /// <summary>
    /// Id задачи в рамках проекта вместе с префиксом.
    /// </summary>
    public string FullProjectTaskId => string.Concat(TaskIdPrefix + "-", ProjectTaskId);

    /// <summary>
    /// Найденный текст.
    /// </summary>
    public string FindText => string.Concat(FullProjectTaskId + " ", Name);

    /// <summary>
    /// Название статуса задачи.
    /// </summary>
    public string? StatusName { get; set; }

    /// <summary>
    /// Название исполнителя задачи.
    /// </summary>
    public string? ExecutorName { get; set; }

    /// <summary>
    /// Id исполнителя.
    /// </summary>
    public long ExecutorId { get; set; }

    /// <summary>
    /// Тип задачи.
    /// </summary>
    public string? TaskTypeName { get; set; }

    /// <summary>
    /// Id типа задачи.
    /// </summary>
    public int TaskTypeId { get; set; }
}