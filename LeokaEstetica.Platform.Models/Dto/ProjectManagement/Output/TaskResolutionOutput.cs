namespace LeokaEstetica.Platform.Models.Dto.ProjectManagement.Output;

/// <summary>
/// Класс выходной модели резолюции задач.
/// </summary>
public class TaskResolutionOutput
{
    /// <summary>
    /// PK.
    /// </summary>
    public int ResolutionId { get; set; }
    
    /// <summary>
    /// Название резолюции.
    /// </summary>
    public string ResolutionName { get; set; }

    /// <summary>
    /// Системное название резолюции.
    /// </summary>
    public string ResolutionSysName { get; set; }
    
    /// <summary>
    /// Порядковый номер.
    /// </summary>
    public int Position { get; set; }
}