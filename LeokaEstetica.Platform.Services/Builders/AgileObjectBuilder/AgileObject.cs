namespace LeokaEstetica.Platform.Services.Builders.AgileObjectBuilder;

/// <summary>
/// Объект Agile.
/// </summary>
public class AgileObject
{
    /// <summary>
    /// Метод строит нужный тип объекта.
    /// </summary>
    /// <param name="builder">Строитель, который занимается построением нужного объекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Результирующая модель.</returns>
    public async Task BuildAsync(AgileObjectBuilder builder, long projectTaskId, long projectId)
    {
        await builder.CreateProjectManagmentTaskAsync(projectTaskId, projectId);
        await builder.InitObjectAsync(projectTaskId, projectId);
        await builder.FillAuthorNameAsync();
        await builder.FillExecutorNameAsync();
        await builder.FillExecutorAvatarAsync();
        await builder.FillWatcherNamesAsync();
        await builder.FillTagIdsAsync();
        await builder.FillTaskTypeNameAsync();
        await builder.FillTaskStatusNameAsync();
        await builder.FillResolutionNameAsync();
        await builder.FillPriorityNameAsync();
        await builder.FillEpicIdAndEpicNameAsync();
        await builder.FillSprintIdAndSprintNameAsync();
    }
}