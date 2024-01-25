namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели изменения описания задачи.
/// </summary>
public class ChangeTaskDetailsInput : BaseChangeTaskInput
{
    /// <summary>
    /// Новое описание задачи.
    /// </summary>
    public string ChangedTaskDetails { get; set; }
}