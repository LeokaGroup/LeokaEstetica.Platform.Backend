using LeokaEstetica.Platform.Models.Dto.Base.ProjectManagement;

namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели завершения спринта.
/// </summary>
public class ManualCompleteSprintInput : SprintInput
{
    /// <summary>
    /// Данные действий пользователя, если они есть.
    /// </summary>
    public BaseNeedSprintAction? NeedSprintAction { get; set; }

    /// <summary>
    /// Признак обработанного действия пользователем (т.е. если он выбрал действие уже).
    /// </summary>
    public bool IsProcessedAction { get; set; }
    
    /// <summary>
    /// Список незавершенных задач спринта.
    /// </summary>
    public IEnumerable<long>? NotCompletedSprintTaskIds { get; set; }

    /// <summary>
    /// Id спринта для переноса в него задач.
    /// </summary>
    public long? MoveSprintId { get; set; }

    /// <summary>
    /// Название нового спринта для переноса в него задач.
    /// </summary>
    public string? MoveSprintName { get; set; }
}