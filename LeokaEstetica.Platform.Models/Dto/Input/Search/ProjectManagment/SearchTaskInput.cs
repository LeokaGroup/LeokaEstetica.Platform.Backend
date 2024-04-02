namespace LeokaEstetica.Platform.Models.Dto.Input.Search.ProjectManagment;

/// <summary>
/// Класс входной модели поиска задач.
/// </summary>
public class SearchTaskInput : BaseSearchAgileObjectInput
{
    /// <summary>
    /// Id проектов, по которым искать.
    /// </summary>
    public IEnumerable<long> ProjectIds { get; set; }
}