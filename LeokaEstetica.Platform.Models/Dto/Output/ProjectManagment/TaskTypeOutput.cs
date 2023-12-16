namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели типов задач.
/// </summary>
public class TaskTypeOutput
{
    /// <summary>
    /// Id типа задачи.
    /// </summary>
    public int TypeId { get; set; }

    /// <summary>
    /// Название типа.
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    /// Системное название типа.
    /// </summary>
    public string TypeSysName { get; set; }
}