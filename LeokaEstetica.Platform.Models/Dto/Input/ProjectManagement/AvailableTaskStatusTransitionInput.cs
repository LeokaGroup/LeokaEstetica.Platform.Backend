namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели доступных переходов задачи.
/// </summary>
public class AvailableTaskStatusTransitionInput
{
    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Id задачи в рамках проекта.
    /// </summary>
    public string ProjectTaskId { get; set; }

    /// <summary>
    /// Тип детализации.
    /// </summary>
    public string TaskDetailType { get; set; }
}