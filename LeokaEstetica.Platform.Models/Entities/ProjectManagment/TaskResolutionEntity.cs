namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сопоставляется с таблицей резолюций задач.
/// </summary>
public class TaskResolutionEntity
{
    public TaskResolutionEntity(string resolutionName, string resolutionSysName)
    {
		ResolutionName = resolutionName;
        ResolutionSysName = resolutionSysName;
	}
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