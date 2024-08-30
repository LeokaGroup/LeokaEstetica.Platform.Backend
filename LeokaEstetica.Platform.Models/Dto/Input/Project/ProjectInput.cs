using LeokaEstetica.Platform.Models.Enums;
using Enum = System.Enum;

namespace LeokaEstetica.Platform.Models.Dto.Input.Project;

/// <summary>
/// Класс входной модели проекта.
/// </summary>
public class ProjectInput
{
	/// <summary>
	/// Конструктор.
	/// </summary>
	/// <param name="projectName">Название проекта.</param>
	/// <param name="projectDetails">Описание проекта.</param>
	/// <param name="projectId">Id проекта.</param>
	/// <param name="projectStage">Стадия проекта.</param>
	/// <param name="userId">Id пользователя.</param>
	/// <param name="isPublic">Видимость проекта.</param>
	public ProjectInput(string projectName, string projectDetails, long? projectId, string projectStage, 
        long? userId,bool isPublic)
    {
        if (projectId.HasValue)
        {
            ProjectId = projectId.Value;
        }

        if (userId.HasValue)
        {
            UserId = userId.Value;
        }
        
        ProjectName = projectName;
        ProjectDetails = projectDetails;
        ProjectStage = projectStage;
        IsPublic = isPublic;
    }

    /// <summary>
    /// Название проекта.
    /// </summary>
    public string ProjectName { get; set; }

    /// <summary>
    /// Описание проекта.
    /// </summary>
    public string ProjectDetails { get; set; }

    /// <summary>
    /// Изображение проекта.
    /// </summary>
    public string? ProjectIcon { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long? ProjectId { get; set; }

    /// <summary>
    /// Стадия проекта.
    /// </summary>
    public string ProjectStage { get; set; }

    /// <summary>
    /// Условия.
    /// </summary>
    public string? Conditions { get; set; }

    /// <summary>
    /// Требования.
    /// </summary>
    public string? Demands { get; set; }
    
    /// <summary>
    /// Аккаунт.
    /// </summary>
    public string? Account { get; set; }

    /// <summary>
    /// Стадия проекта в виде перечисления.
    /// </summary>
    public ProjectStageEnum ProjectStageEnum => Enum.Parse<ProjectStageEnum>(ProjectStage);

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

	/// <summary>
	/// Видимость проекта
	/// </summary>
	public bool IsPublic { get; set; }
}