namespace LeokaEstetica.Platform.Models.Dto.Input.Search.ProjectManagment;

/// <summary>
/// Базовый класс поиска объекта Agile (задачи, ошибки, истории, эпика, спринта).
/// </summary>
public class BaseSearchAgileObjectInput
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
}