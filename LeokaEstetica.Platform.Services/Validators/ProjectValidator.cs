using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Utils;
using LeokaEstetica.Platform.Logs.Abstractions;
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

    /// <summary>
    /// Метод валидарует входные данные при создании вакансии проекта.
    /// </summary>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyText">Описание вакансии.</param>
    /// <param name="result">Результирующая модель. Тут не возвращается, так как передана по ссылке сюда.</param>
    public static void ValidateProjectVacancy(string vacancyName, string vacancyText, CreateProjectVacancyOutput result)
    {
        // Если не заполнили название вакансии проекта.
        if (string.IsNullOrEmpty(vacancyName))
        {
            result.Errors.Add(GlobalConfigKeys.ProjectVacancy.EMPTY_PROJECT_VACANCY_NAME);
        }

        // Если не заполнили описание вакансии проекта.
        if (string.IsNullOrEmpty(vacancyText))
        {
            result.Errors.Add(GlobalConfigKeys.ProjectVacancy.EMPTY_PROJECT_VACANCY_TEXT);
        }
    }
    
    /// <summary>
    /// Метод валидирует входные параметры при создании вакансии проекта.
    /// </summary>
    /// <param name="result">Выходные данные. Писать ошибки валидации туда, если будут.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyText">Описание вакансии.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    public static void ValidateCreateProjectVacancy(ref CreateProjectVacancyOutput result, string vacancyName, string vacancyText, string account)
    {
        var logger = AutoFac.Resolve<ILogService>();
        
        // Если не заполнили название вакансии проекта.
        if (string.IsNullOrEmpty(vacancyName))
        {
            var ex = new ArgumentNullException(GlobalConfigKeys.ProjectVacancy.EMPTY_PROJECT_VACANCY_NAME);
            result.Errors.Add(GlobalConfigKeys.ProjectVacancy.EMPTY_PROJECT_VACANCY_NAME);
            logger.LogError(ex);
        }
        
        // Если не заполнили описание вакансии проекта.
        if (string.IsNullOrEmpty(vacancyText))
        {
            var ex = new ArgumentNullException(GlobalConfigKeys.ProjectVacancy.EMPTY_PROJECT_VACANCY_TEXT);
            result.Errors.Add(GlobalConfigKeys.ProjectVacancy.EMPTY_PROJECT_VACANCY_TEXT);
            logger.LogError(ex);
        }
        
        if (string.IsNullOrEmpty(account))
        {
            var ex = new ArgumentNullException($"Не передан аккаунт пользователя.");
            logger.LogError(ex);
        }
    }
}