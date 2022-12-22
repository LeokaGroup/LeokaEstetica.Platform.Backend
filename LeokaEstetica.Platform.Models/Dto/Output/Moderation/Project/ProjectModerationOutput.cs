namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;

/// <summary>
/// Класс выходной модели проектов для модерации.
/// </summary>
public class ProjectModerationOutput
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
    /// Дата создания проекта.
    /// </summary>
    public string DateModeration { get; set; }

    /// <summary>
    /// FK на проект модерации.
    /// </summary>
    public UserProjectModerationOutput UserProject { get; set; }
}