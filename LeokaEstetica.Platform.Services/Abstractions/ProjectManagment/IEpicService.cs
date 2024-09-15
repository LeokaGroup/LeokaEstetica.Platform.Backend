namespace LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;

/// <summary>
/// Абстракция сервиса эпика.
/// </summary>
public interface IEpicService
{
    /// <summary>
    /// Метод исключает задачи из эпика.
    /// </summary>
    /// <param name="epicId">Id эпика.</param>
    /// <param name="projectTaskIds">Список Id задач в рамках проекта, которые нужно исключить из эпика.</param>
    Task ExcludeEpicTasksAsync(long epicId, IEnumerable<string>? projectTaskIds);
}