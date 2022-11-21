using LeokaEstetica.Platform.Models.Dto.Output.Project;

namespace LeokaEstetica.Platform.Services.Validators;

/// <summary>
/// Класс валидатора проектов.
/// </summary>
public static class ProjectValidator
{
    /// <summary>
    /// Если не заполнили название проекта.
    /// </summary>
    private const string EMPTY_PROJECT_NAME = "Не заполнено название проекта.";
    
    /// <summary>
    /// Если не заполнили описание проекта.
    /// </summary>
    private const string EMPTY_PROJECT_DETAILS = "Не заполнено описание проекта.";

    /// <summary>
    /// Метод валидация проекта при его создании.
    /// </summary>
    /// <param name="projectName"></param>
    /// <param name="projectDetails"></param>
    /// <param name="result">Результирующая модель. Тут не возвращается, так как передана по ссылке сюда.</param>
    public static void ValidateCreateProject(string projectName, string projectDetails, ref CreateProjectOutput result)
    {
        if (string.IsNullOrEmpty(projectName))
        {
            result.Errors.Add(EMPTY_PROJECT_NAME);
        }
        
        if (string.IsNullOrEmpty(projectDetails))
        {
            result.Errors.Add(EMPTY_PROJECT_DETAILS);
        }
    }

    /// <summary>
    /// Метод валидация проекта при его изменении.
    /// </summary>
    /// <param name="projectName"></param>
    /// <param name="projectDetails"></param>
    /// <param name="result">Результирующая модель. Тут не возвращается, так как передана по ссылке сюда.</param>
    public static void ValidateUpdateProject(string projectName, string projectDetails, ref UpdateProjectOutput result)
    {
        if (string.IsNullOrEmpty(projectName))
        {
            result.Errors.Add(EMPTY_PROJECT_NAME);
        }
        
        if (string.IsNullOrEmpty(projectDetails))
        {
            result.Errors.Add(EMPTY_PROJECT_DETAILS);
        }
    }
}