namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс результата выходной модели задач эпика.
/// </summary>
public class EpicTaskResult
{
    /// <summary>
    /// Список задач эпика.
    /// </summary>
    public IEnumerable<EpicTaskOutput> EpicTasks { get; set; }

    /// <summary>
    /// Кол-во всего.
    /// </summary>
    public int Total => EpicTasks.Count();
}