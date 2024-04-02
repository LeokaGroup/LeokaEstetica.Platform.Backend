using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;

namespace LeokaEstetica.Platform.Services.Strategies.ProjectManagement.SprintTaskSearch;

/// <summary>
/// Класс представляет семейство алгоритмов для поиска Agile-объекта (задачи, ошибки, эпика, истории)
/// для включения их в спринт или в эпик.
/// </summary>
internal class BaseSearchAgileObjectAlgorithm
{
    /// <summary>
    /// Метод ищет задачи, истории, эпики, ошибки по Id задачи в рамках проекта.
    /// </summary>
    /// <param name="strategy">Стратегия поиска.</param>
    /// <param name="inviteText">Текст для приглашения.</param>
    /// <param name="templateId">Id шаблона.</param>
    /// <param name="searchAgileObjectType">Тип поиска объекта (чтобы понимать, что искать).</param>
    /// <returns>Найденные задачи, истории, эпики, ошибки.</returns>
    internal async Task<IEnumerable<SearchAgileObjectOutput>> SearchSearchAgileObjectByProjectTaskIdAsync(
        BaseSearchAgileObjectStrategy strategy, long projectTaskId, long projectId, int templateId,
        SearchAgileObjectTypeEnum searchAgileObjectType)
    {
        var result = await strategy.SearchAgileObjectByProjectTaskIdAsync(projectTaskId, projectId, templateId,
                searchAgileObjectType);

        return result;
    }

    /// <summary>
    /// Метод ищет задачи, истории, эпики, ошибки по названию задачи, эпика, истории, ошибки.
    /// </summary>
    /// <param name="taskName">Название задачи, эпика, истории, ошибки./</param>
    /// <param name="projectId">Id проекта./</param>
    /// <param name="templateId">Id шаблона.</param>
    /// <param name="searchAgileObjectType">Тип поиска объекта (чтобы понимать, что искать).</param>
    /// <returns>Найденные задачи, истории, эпики, ошибки.</returns>
    public async Task<IEnumerable<SearchAgileObjectOutput>> SearchSearchAgileObjectByTaskNameAsync(
        BaseSearchAgileObjectStrategy strategy, string taskName, long projectId, int templateId,
        SearchAgileObjectTypeEnum searchAgileObjectType)
    {
        var result = await strategy.SearchAgileObjectByTaskNameAsync(taskName, projectId, templateId,
            searchAgileObjectType);

        return result;
    }

    /// <summary>
    /// Метод ищет задачи, истории, эпики, ошибки по совпадении в описании задачи, эпика, истории, ошибки.
    /// </summary>
    /// <param name="taskDescription">Описание задачи, эпика, истории, ошибки./</param>
    /// <param name="projectId">Id проекта./</param>
    /// <param name="templateId">Id шаблона.</param>
    /// <param name="searchAgileObjectType">Тип поиска объекта (чтобы понимать, что искать).</param>
    /// <returns>Найденные задачи, истории, эпики, ошибки.</returns>
    internal async Task<IEnumerable<SearchAgileObjectOutput>> SearchSearchAgileObjectByTaskDescriptionAsync(
        BaseSearchAgileObjectStrategy strategy, string taskDescription, long projectId, int templateId,
        SearchAgileObjectTypeEnum searchAgileObjectType)
    {
        var result = await strategy.SearchAgileObjectByTaskDescriptionAsync(taskDescription, projectId,
            templateId, searchAgileObjectType);

        return result;
    }
}