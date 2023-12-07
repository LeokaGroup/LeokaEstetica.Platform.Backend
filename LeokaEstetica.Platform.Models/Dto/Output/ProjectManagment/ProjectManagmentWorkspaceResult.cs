using LeokaEstetica.Platform.Models.Dto.Output.Template;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс результата рабочего пространства.
/// </summary>
public class ProjectManagmentWorkspaceResult
{
    /// <summary>
    /// Список статусов.
    /// </summary>
    public IEnumerable<ProjectManagmentTaskStatusTemplateOutput> ProjectManagmentTaskStatuses { get; set; }
}