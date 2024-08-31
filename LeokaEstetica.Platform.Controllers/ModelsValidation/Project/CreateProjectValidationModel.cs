using LeokaEstetica.Platform.Models.Dto.Input.Project;

namespace LeokaEstetica.Platform.Controllers.ModelsValidation.Project;

/// <summary>
/// Класс валидации создания проекта.
/// </summary>
public class CreateProjectValidationModel : CreateProjectInput
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectDetails">Описание проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectStage">Стадия проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    public CreateProjectValidationModel(string projectName, string projectDetails, long projectId, string projectStage,
        long userId, bool isPublic)
        : base(projectName, projectDetails, projectId, projectStage, userId, isPublic)
    {
    }
}