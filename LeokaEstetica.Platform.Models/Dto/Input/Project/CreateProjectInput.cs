using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Input.Project;

/// <summary>
/// Класс входной модели для создания нового проекта.
/// </summary>
public class CreateProjectInput : ProjectInput, IFrontError
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
    public CreateProjectInput(string projectName, string projectDetails, long? projectId, string projectStage,
        long? userId, bool isPublic)
        : base(projectName, projectDetails, projectId, projectStage, userId, isPublic)
    {
    }
    
    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure>? Errors { get; set; }

    /// <summary>
    /// Id компании, к которой привязывается проект.
    /// Если передали, значит нужно привязать проект с указанной компанией.
    /// </summary>
    public long? CompanyId { get; set; }
}