using LeokaEstetica.Platform.Models.Entities.ProjectManagment;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели истории.
/// </summary>
public class UserStoryOutput : UserStoryEntity
{
    /// <summary>
    /// Название статуса истории.
    /// </summary>
    public string StoryStatusName { get; set; }

    /// <summary>
    /// Названия наблюдателей.
    /// </summary>
    public List<string> WatcherNames { get; set; }

    /// <summary>
    /// Список названий тегов истории.
    /// </summary>
    public List<string> TagNames { get; set; }

    /// <summary>
    /// Название исполнителя истории, если назначался.
    /// </summary>
    public string ExecutorName { get; set; }
}