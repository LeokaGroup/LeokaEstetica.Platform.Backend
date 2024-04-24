namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели изменения статуса задачи.
/// </summary>
public class ChangeTaskStatusInput
{
    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Id статуса, на который нужно обновить.
    /// </summary>
    public string ChangeStatusId { get; set; }

    /// <summary>
    /// Id задачи (здесь имеется в виду Id задачи в рамках проекта).
    /// </summary>
    public string TaskId { get; set; }

    /// <summary>
    /// Тип детализации.
    /// </summary>
    public string TaskDetailType { get; set; }
}