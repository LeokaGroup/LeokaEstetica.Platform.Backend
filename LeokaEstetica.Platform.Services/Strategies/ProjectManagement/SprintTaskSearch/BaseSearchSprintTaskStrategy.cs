using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;

namespace LeokaEstetica.Platform.Services.Strategies.ProjectManagement.SprintTaskSearch;

/// <summary>
/// Базовый класс поиска задач для включения их в спринт.
/// </summary>
internal abstract class BaseSearchSprintTaskStrategy
{
    protected readonly IProjectManagmentRepository ProjectManagmentRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectManagmentRepository">Репозиторий модуля УП.</param>
    protected BaseSearchSprintTaskStrategy(IProjectManagmentRepository projectManagmentRepository)
    {
        ProjectManagmentRepository = projectManagmentRepository;
    }

    /// <summary>
    /// Метод ищет задачи, истории, эпики, ошибки по Id задачи в рамках проекта.
    /// </summary>
    /// <param name="projectTaskId">Id задачи в рамках проекта./</param>
    /// <param name="projectId">Id проекта./</param>
    /// <returns>Найденные задачи, истории, эпики, ошибки.</returns>
    public abstract Task<IEnumerable<SearchTaskOutput>> SearchIncludeSprintTaskByProjectTaskIdAsync(
        long projectTaskId, long projectId);
    
    /// <summary>
    /// Метод ищет задачи, истории, эпики, ошибки по названию задачи, эпика, истории, ошибки.
    /// </summary>
    /// <param name="taskName">Название задачи, эпика, истории, ошибки./</param>
    /// <param name="projectId">Id проекта./</param>
    /// <returns>Найденные задачи, истории, эпики, ошибки.</returns>
    public abstract Task<IEnumerable<SearchTaskOutput>> SearchIncludeSprintTaskByTaskNameAsync(string taskName,
        long projectId);
    
    /// <summary>
    /// Метод ищет задачи, истории, эпики, ошибки по совпадении в описании задачи, эпика, истории, ошибки.
    /// </summary>
    /// <param name="taskDescription">Описание задачи, эпика, истории, ошибки./</param>
    /// <param name="projectId">Id проекта./</param>
    /// <returns>Найденные задачи, истории, эпики, ошибки.</returns>
    public abstract Task<IEnumerable<SearchTaskOutput>> SearchIncludeSprintTaskByTaskDescriptionAsync(
        string taskDescription, long projectId);
}