using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Input.Project;

/// <summary>
/// Класс входной модели для обновления проекта.
/// </summary>
public class UpdateProjectInput : ProjectInput, IFrontError
{
    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure>? Errors { get; set; }

	/// <summary>
	/// Конструктор.
	/// </summary>
	/// <param name="projectName">Название проекта.</param>
	/// <param name="projectDetails">Описание проекта.</param>
	/// <param name="projectId">Id проекта.</param>
	/// <param name="projectStage">Стадия проекта.</param>
	/// <param name="userId">Id пользователя.</param>
	/// <param name="isPublic">Видимость проекта.</param>
	public UpdateProjectInput(string projectName, string projectDetails, long projectId, string projectStage,
        long? userId, bool isPublic)
        : base(projectName, projectDetails, projectId, projectStage, userId, isPublic)
    {
    }
}