namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели изменения описания задачи.
/// </summary>
public class ChangeTaskDetailsInput
{
    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Id задачи (здесь имеется в виду Id задачи в рамках проекта).
    /// </summary>
    public long TaskId { get; set; }

    /// <summary>
    /// Новое описание задачи.
    /// </summary>
    public string ChangedTaskDetails { get; set; }
}