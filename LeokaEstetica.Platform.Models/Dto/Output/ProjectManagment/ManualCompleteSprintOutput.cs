using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели завершения спринта.
/// </summary>
public class ManualCompleteSprintOutput : ManualCompleteSprintInput
{
    /// <summary>
    /// Список незавершенных задач спринта.
    /// </summary>
    public IEnumerable<long>? NotCompletedSprintTaskIds { get; set; }
}