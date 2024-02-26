namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели файлов задачи.
/// </summary>
public class ProjectTaskFileInput
{
    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Id задачи (в рамках проекта).
    /// </summary>
    public string TaskId { get; set; }
}