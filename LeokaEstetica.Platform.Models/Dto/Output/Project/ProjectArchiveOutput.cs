namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели архива проектов.
/// </summary>
public class ProjectArchiveOutput
{
   /// <summary>
   /// Id проекта.
   /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Название проекта.
    /// </summary>
    public string ProjectName { get; set; }

    /// <summary>
    /// Дата архива.
    /// </summary>
    public string DateArchived { get; set; }
}
