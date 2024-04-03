using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;

namespace LeokaEstetica.Platform.Services.Strategies.ProjectManagement.SprintTaskSearch;

/// <summary>
/// Базовый класс поиска задач для включения их в спринт.
/// </summary>
internal abstract class BaseSearchAgileObjectStrategy
{
    protected readonly IProjectManagmentRepository ProjectManagmentRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectManagmentRepository">Репозиторий модуля УП.</param>
    internal BaseSearchAgileObjectStrategy(IProjectManagmentRepository projectManagmentRepository)
    {
        ProjectManagmentRepository = projectManagmentRepository;
    }

    /// <summary>
    /// Метод находит Agile-объекты. Это может быть задача, эпик, история, ошибка, спринт.
    /// </summary>
    /// <param name="projectTaskId">Id задачи в рамках проекта./</param>
    /// <param name="projectId">Id проекта./</param>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Найденные задачи, истории, эпики, ошибки.</returns>
    internal abstract Task<IEnumerable<SearchTaskOutput>> SearchAgileObjectByObjectIdAsync(
        long projectTaskId, long projectId, int templateId);
    
    /// <summary>
    /// Метод находит Agile-объекты. Это может быть задача, эпик, история, ошибка, спринт.
    /// </summary>
    /// <param name="taskName">Название задачи, эпика, истории, ошибки./</param>
    /// <param name="projectId">Id проекта./</param>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Найденные задачи, истории, эпики, ошибки.</returns>
    internal abstract Task<IEnumerable<SearchTaskOutput>> SearchAgileObjectByObjectNameAsync(string taskName,
        long projectId, int templateId);
    
    /// <summary>
    /// Метод находит Agile-объекты. Это может быть задача, эпик, история, ошибка, спринт.
    /// </summary>
    /// <param name="taskDescription">Описание задачи, эпика, истории, ошибки./</param>
    /// <param name="projectId">Id проекта./</param>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Найденные задачи, истории, эпики, ошибки.</returns>
    internal abstract Task<IEnumerable<SearchTaskOutput>> SearchAgileObjectByObjectDescriptionAsync(
        string taskDescription, long projectId, int templateId);
}