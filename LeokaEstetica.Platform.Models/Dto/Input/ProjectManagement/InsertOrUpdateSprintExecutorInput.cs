namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели проставления/обновления исполнителя спринта (ответственный за выполнение спринта).
/// </summary>
public class InsertOrUpdateSprintExecutorInput : SprintInput
{
    /// <summary>
    /// Id исполнителя спринта.
    /// </summary>
    public long ExecutorId { get; set; }
}