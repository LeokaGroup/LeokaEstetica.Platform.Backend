namespace LeokaEstetica.Platform.Models.Entities.Project;

/// <summary>
/// Класс сопоставляется с таблицей стадий проектов пользователя Projects.UserProjectsStages.
/// </summary>
public class UserProjectStageEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long UserProjectStageId { get; set; }

    public long ProjectId { get; set; }

    /// <summary>
    /// FK на таблицу проектов пользователя.
    /// </summary>
    public UserProjectEntity UserProject { get; set; }

    /// <summary>
    /// Id стадии проекта.
    /// </summary>
    public int StageId { get; set; }
}