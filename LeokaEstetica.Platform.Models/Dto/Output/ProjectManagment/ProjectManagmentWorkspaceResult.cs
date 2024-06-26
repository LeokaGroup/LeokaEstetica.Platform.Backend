namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс результата рабочего пространства.
/// </summary>
public class ProjectManagmentWorkspaceResult : BaseProjectManagementTask
{
    /// <summary>
    /// Признак доступа.
    /// </summary>
    public bool IsAccess { get; set; }
}