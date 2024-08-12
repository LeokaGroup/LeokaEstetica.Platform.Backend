using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;

namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели проектов пользователя.
/// </summary>
public class UserProjectOutput
{
    /// <summary>
    /// Название проекта.
    /// </summary>
    public string? ProjectName { get; set; }

	/// <summary>
	/// Краткое описание проекта.
	/// </summary>
	public string? ProjectDetails { get; set; }
	
	/// <summary>
    /// Изображение проекта.
    /// </summary>
    public string? ProjectIcon { get; set; }

    /// <summary>
    /// Название статуса проекта.
    /// </summary>
    public string? ProjectStatusName { get; set; }
    
    /// <summary>
    /// Системное название статуса проекта.
    /// </summary>
    public string? ProjectStatusSysName { get; set; }

    /// <summary>
    /// Код проекта.
    /// </summary>
    public Guid ProjectCode { get; set; }

    /// <summary>
    /// PK.
    /// </summary>
    public long ProjectId { get; set; }
    
    /// <summary>
    /// Список замечаний проекта.
    /// </summary>
    public IEnumerable<ProjectRemarkOutput>? ProjectRemarks { get; set; }

    /// <summary>
    /// Полное описание проекта для тултипа.
    /// </summary>
    public string? ProjectDetailsTooltip { get; set; }
}