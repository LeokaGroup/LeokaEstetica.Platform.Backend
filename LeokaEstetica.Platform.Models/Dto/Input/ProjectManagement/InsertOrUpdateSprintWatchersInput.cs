namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели проставления/обновления наблюдателей спринта.
/// </summary>
public class InsertOrUpdateSprintWatchersInput : SprintInput
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="watcherIds">Список Id наблюдателей спринта.</param>
    public InsertOrUpdateSprintWatchersInput(IEnumerable<long> watcherIds)
    {
        WatcherIds = watcherIds;
    }

    /// <summary>
    /// Id исполнителя спринта.
    /// </summary>
    public IEnumerable<long> WatcherIds { get; set; }
}