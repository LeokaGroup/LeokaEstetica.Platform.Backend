using LeokaEstetica.Platform.Core.Enums;

namespace LeokaEstetica.Platform.Services.Builders.AgileObjectBuilder;

/// <summary>
/// Объект Agile.
/// </summary>
internal class AgileObject
{
    /// <summary>
    /// Метод строит нужный тип объекта.
    /// </summary>
    /// <param name="builder">Строитель, который занимается построением нужного объекта.</param>
    /// <param name="taskDetailType">Тип детализации.</param>
    /// <returns>Результирующая модель.</returns>
    public async Task BuildAsync(BaseAgileObjectBuilder builder, TaskDetailTypeEnum taskDetailType)
    {
        await builder.CreateProjectManagmentTaskAsync();
        await builder.InitObjectAsync();
        await builder.FillAuthorNameAsync();
        await builder.FillExecutorNameAsync();
        await builder.FillExecutorAvatarAsync();
        await builder.FillWatcherNamesAsync();
        await builder.FillTagIdsAsync();
        await builder.FillTaskTypeNameAsync();
        await builder.FillTaskStatusNameAsync();

        if (taskDetailType is TaskDetailTypeEnum.Task or TaskDetailTypeEnum.Error)
        {
            await builder.FillResolutionNameAsync();
            await builder.FillEpicIdAndEpicNameAsync();
            await builder.FillSprintIdAndSprintNameAsync();
        }
        
        await builder.FillPriorityNameAsync();

        if (taskDetailType == TaskDetailTypeEnum.Epic)
        {
            await builder.FillEpicTasksAsync();
        }
    }
}