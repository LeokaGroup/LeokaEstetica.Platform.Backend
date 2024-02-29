namespace LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;

/// <summary>
/// Класс выходной модели поиска задач.
/// </summary>
public class SearchTaskOutput
{
    /// <summary>
    /// Название задачи.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Id задачи в рамках проекта.
    /// </summary>
    public long ProjectTaskId { get; set; }
    
    /// <summary>
    /// Префикс номера задачи.
    /// </summary>
    public string TaskIdPrefix { get; set; }

    /// <summary>
    /// Id задачи в рамках проекта вместе с префиксом.
    /// </summary>
    public string FullProjectTaskId => string.Concat(TaskIdPrefix + "-", ProjectTaskId);

    /// <summary>
    /// Найденный текст.
    /// </summary>
    public string FindText => string.Concat(FullProjectTaskId + " ", Name);
}