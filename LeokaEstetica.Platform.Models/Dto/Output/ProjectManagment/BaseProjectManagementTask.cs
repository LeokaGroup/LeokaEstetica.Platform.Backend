using LeokaEstetica.Platform.Models.Dto.Output.Template;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Базовый класс статусов задач.
/// </summary>
public class BaseProjectManagementTask
{
    /// <summary>
    /// Список статусов.
    /// </summary>
    public IEnumerable<ProjectManagmentTaskStatusTemplateOutput> ProjectManagmentTaskStatuses { get; set; }

    /// <summary>
    /// Стратегия представления.
    /// </summary>
    public string Strategy { get; set; }
}