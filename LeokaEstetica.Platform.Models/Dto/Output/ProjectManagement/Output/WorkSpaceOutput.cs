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

    /// <summary>
    /// Признак владельца проекта.
    /// </summary>
    public bool IsOwner { get; set; }

    /// <summary>
    /// Признак уже настроенного названия проекта и префикса (может делать лишь владелец).
    /// </summary>
    public bool IsSetupProjectNameAndPrefix { get; set; }

    /// <summary>
    /// Id компании.
    /// </summary>
    public long CompanyId { get; set; }

    /// <summary>
    /// Название компании.
    /// </summary>
    public string? CompanyName { get; set; }
}