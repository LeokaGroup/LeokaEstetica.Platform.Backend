namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели приоритетов задачи.
/// </summary>
public class TaskPriorityOutput
{
    /// <summary>
    /// Id приоритета.
    /// </summary>
    public int PriorityId { get; set; }

    /// <summary>
    /// Название приоритета.
    /// </summary>
    public string PriorityName { get; set; }
    
    /// <summary>
    /// Системное название приоритета.
    /// </summary>
    public string PrioritySysName { get; set; }
}