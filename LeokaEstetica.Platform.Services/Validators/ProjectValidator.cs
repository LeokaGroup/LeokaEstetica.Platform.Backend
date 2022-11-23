using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Output.Project;

namespace LeokaEstetica.Platform.Services.Validators;

/// <summary>
/// Класс валидатора проектов.
/// </summary>
public static class ProjectValidator
{
    /// <summary>
    /// Метод валидация проекта при его создании.
    /// </summary>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectDetails">Описание проекта.</param>
    /// <param name="result">Результирующая модель. Тут не возвращается, так как передана по ссылке сюда.</param>
    public static void ValidateCreateProject(string projectName, string projectDetails, ref CreateProjectOutput result)
    {
        if (string.IsNullOrEmpty(projectName))
        {
            result.Errors.Add(GlobalConfigKeys.ProjectMode.EMPTY_PROJECT_NAME);
        }
        
        if (string.IsNullOrEmpty(projectDetails))
        {
            result.Errors.Add(GlobalConfigKeys.ProjectMode.EMPTY_PROJECT_DETAILS);
        }
    }

    /// <summary>
    /// Метод валидация проекта при его изменении.
    /// </summary>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectDetails">Описание проекта.</param>
    /// <param name="result">Результирующая модель. Тут не возвращается, так как передана по ссылке сюда.</param>
    public static void ValidateUpdateProject(string projectName, string projectDetails, ref UpdateProjectOutput result)
    {
        if (string.IsNullOrEmpty(projectName))
        {
            result.Errors.Add(GlobalConfigKeys.ProjectMode.EMPTY_PROJECT_NAME);
        }
        
        if (string.IsNullOrEmpty(projectDetails))
        {
            result.Errors.Add(GlobalConfigKeys.ProjectMode.EMPTY_PROJECT_DETAILS);
        }
    }
    
    /// <summary>
    /// Метод валидация проекта при его получении для просмотра или изменения.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="mode">Режим. Чтение или изменение.</param>
    /// <param name="result">Результирующая модель. Тут не возвращается, так как передана по ссылке сюда.</param>
    public static void ValidateGetProject(long projectId, ModeEnum mode, ref ProjectOutput result)
    {
        if (projectId <= 0)
        {
            result.Errors.Add(GlobalConfigKeys.ProjectMode.NOT_VALID_PROJECT_ID + projectId);
        }
        
        if (mode == ModeEnum.None)
        {
            result.Errors.Add(GlobalConfigKeys.ProjectMode.EMPTY_MODE + mode);
        }
    }
}