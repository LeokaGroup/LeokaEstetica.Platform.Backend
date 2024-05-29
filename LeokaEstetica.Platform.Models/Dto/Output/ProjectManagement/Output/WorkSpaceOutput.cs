namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;

/// <summary>
/// Класс выходной модели раб.пространств.
/// </summary>
public class WorkSpaceOutput
{
    /// <summary>
    /// Id раб.пространства.
    /// </summary>
    public long WorkSpaceId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Название проекта из УП.
    /// </summary>
    public string? ProjectManagementName { get; set; }
}