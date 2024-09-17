namespace LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;

/// <summary>
/// Абстракция репозитория эпиков.
/// </summary>
public interface IEpicRepository
{
    /// <summary>
    /// Метод исключает задачи из эпика.
    /// </summary>
    /// <param name="epicId">Id эпика.</param>
    /// <param name="projectTaskIds">Список Id задач, которые нужно исключить из эпика.</param>
    Task ExcludeEpicTasksAsync(long epicId, IEnumerable<long>? epicTaskIds);
}