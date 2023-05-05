namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;

/// <summary>
/// Класс выходной модели для замечаний таблицы проектов.
/// </summary>
public class ProjectRemarkTableOutput
{
    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Название проекта.
    /// </summary>
    public string ProjectName { get; set; }
}