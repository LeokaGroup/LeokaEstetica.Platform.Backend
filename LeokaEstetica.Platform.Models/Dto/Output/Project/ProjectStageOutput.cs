namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели стадий проекта.
/// </summary>
public class ProjectStageOutput
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