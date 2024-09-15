namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели исключения задач из эпика.
/// </summary>
public class ExcludeEpicTaskInput
{
    /// <summary>
    /// Id эпика.
    /// </summary>
    public long EpicId { get; set; }

    /// <summary>
    /// Список Id задач в рамках проекта, которые нужно исключить из эпика.
    /// </summary>
    public IEnumerable<string>? ProjectTaskIds { get; set; }
}