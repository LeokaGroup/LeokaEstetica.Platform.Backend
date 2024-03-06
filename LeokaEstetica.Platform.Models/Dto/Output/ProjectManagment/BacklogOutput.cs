using LeokaEstetica.Platform.Models.Dto.Output.Template;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели бэклога.
/// </summary>
public class BacklogOutput
{
    /// <summary>
    /// Список задач бэклога.
    /// </summary>
    public IEnumerable<ProjectManagmentTaskOutput> Tasks { get; set; }
}