using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;

namespace LeokaEstetica.Platform.Services.Strategies.ProjectManagement.SprintTaskSearch;

/// <summary>
/// Класс представляет семейство алгоритмов для поиска задач для включения их в спринт.
/// </summary>
internal class BaseSearchAgileObjectAlgorithm
{
    /// <summary>
    /// Метод ищет задачи, истории, эпики, ошибки по Id задачи в рамках проекта.
    /// </summary>
    /// <param name="strategy">Стратегия поиска.</param>
    /// <param name="inviteText">Текст для приглашения.</param>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Найденные задачи, истории, эпики, ошибки.</returns>
    internal async Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByObjectIdAsync(
        BaseSearchAgileObjectStrategy strategy, long projectTaskId, long projectId, int templateId)
    {
        var result = await strategy.SearchAgileObjectByObjectIdAsync(projectTaskId, projectId, templateId);

        return result;
    }

    /// <summary>
    /// Метод ищет задачи, истории, эпики, ошибки по названию задачи, эпика, истории, ошибки.
    /// </summary>
    /// <param name="taskName">Название задачи, эпика, истории, ошибки./</param>
    /// <param name="projectId">Id проекта./</param>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Найденные задачи, истории, эпики, ошибки.</returns>
    internal async Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByObjectNameAsync(
        BaseSearchAgileObjectStrategy strategy, string taskName, long projectId, int templateId)
    {
        var result = await strategy.SearchAgileObjectByObjectNameAsync(taskName, projectId, templateId);

        return result;
    }

    /// <summary>
    /// Метод ищет задачи, истории, эпики, ошибки по совпадении в описании задачи, эпика, истории, ошибки.
    /// </summary>
    /// <param name="taskDescription">Описание задачи, эпика, истории, ошибки./</param>
    /// <param name="projectId">Id проекта./</param>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Найденные задачи, истории, эпики, ошибки.</returns>
    internal async Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectByObjectDescriptionAsync(
        BaseSearchAgileObjectStrategy strategy, string taskDescription, long projectId, int templateId)
    {
        var result = await strategy.SearchAgileObjectByObjectDescriptionAsync(taskDescription, projectId,
            templateId);

        return result;
    }
}