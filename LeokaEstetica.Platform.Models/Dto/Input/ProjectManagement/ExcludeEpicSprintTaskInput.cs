namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели исключения задач из эпика или спринта.
/// </summary>
public class ExcludeEpicSprintTaskInput
{
    /// <summary>
    /// Id эпика или спринта.
    /// </summary>
    public long EpicSprintId { get; set; }

    /// <summary>
    /// Список Id задач в рамках проекта, которые нужно исключить из эпика или спринта.
    /// </summary>
    public IEnumerable<string>? ProjectTaskIds { get; set; }
}