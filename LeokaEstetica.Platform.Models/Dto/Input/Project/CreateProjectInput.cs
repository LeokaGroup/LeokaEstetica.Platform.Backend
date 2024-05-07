using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Input.Project;

/// <summary>
/// Класс входной модели для создания нового проекта.
/// </summary>
public class CreateProjectInput : ProjectInput, IFrontError
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
    public CreateProjectInput(string projectName, string projectDetails, long? projectId, string projectStage,
        long? userId)
        : base(projectName, projectDetails, projectId, projectStage, userId)
    {
    }
}