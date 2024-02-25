namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс базовой входной модели изменения задачи.
/// </summary>
public class BaseChangeTaskInput
{
    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Id задачи (здесь имеется в виду Id задачи в рамках проекта).
    /// </summary>
    public string TaskId { get; set; }
}