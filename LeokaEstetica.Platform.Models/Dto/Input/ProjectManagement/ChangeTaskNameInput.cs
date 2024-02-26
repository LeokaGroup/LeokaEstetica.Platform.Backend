namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели изменения названия задачи.
/// </summary>
public class ChangeTaskNameInput : BaseChangeTaskInput
{
    /// <summary>
    /// Новое название задачи.
    /// </summary>
    public string ChangedTaskName { get; set; }
}