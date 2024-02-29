namespace LeokaEstetica.Platform.Models.Dto.Input.Search.ProjectManagment;

/// <summary>
/// Класс входной модели поиска задач.
/// </summary>
public class SearchTaskInput
{
    /// <summary>
    /// Поисковый текст.
    /// </summary>
    public string SearchText { get; set; }

    /// <summary>
    /// Признак поиска по названию задачи.
    /// </summary>
    public bool IsByName { get; set; }

    /// <summary>
    /// Признак поиска по Id задачи.
    /// </summary>
    public bool IsById { get; set; }

    /// <summary>
    /// Признак поиска по описанию задачи.
    /// </summary>
    public bool IsByDescription { get; set; }

    /// <summary>
    /// Id проектов, по которым искать.
    /// </summary>
    public IEnumerable<long> ProjectIds { get; set; }
}