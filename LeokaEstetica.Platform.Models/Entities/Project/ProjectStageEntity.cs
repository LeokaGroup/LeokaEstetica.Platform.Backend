namespace LeokaEstetica.Platform.Models.Entities.Project;

/// <summary>
/// Класс сопоставляется с таблицей стадий проекта Projects.ProjectStages.
/// </summary>
public class ProjectStageEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int StageId { get; set; }

    /// <summary>
    /// Название стадии проекта.
    /// </summary>
    public string StageName { get; set; }

    /// <summary>
    /// Системное название стадии проекта.
    /// </summary>
    public string StageSysName { get; set; }

    /// <summary>
    /// Позиция в списке.
    /// </summary>
    public int Position { get; set; }
}