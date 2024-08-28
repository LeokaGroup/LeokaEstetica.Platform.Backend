using LeokaEstetica.Platform.Models.Dto.Base.ProjectManagement;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс системных статусов историй, эпиков.
/// </summary>
public class StoryAndEpicSystemStatusOutput : BaseTaskStatusOutput
{
    /// <summary>
    /// Id истории или эпика.
    /// </summary>
    public long StoryEpicId { get; set; }
}