using LeokaEstetica.Platform.Models.Dto.Output.Template;

namespace LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;

/// <summary>
/// Абстракция сервиса шаблонов проекта.
/// </summary>
public interface IProjectManagementTemplateService
{
    /// <summary>
    /// Метод получает список шаблонов задач, которые пользователь может выбрать перед переходом в рабочее пространство.
    /// </summary>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Список шаблонов задач.</returns>
    Task<IEnumerable<ProjectManagmentTaskTemplateResult>> GetProjectManagmentTemplatesAsync(long? templateId);

    /// <summary>
    /// Метод проставляет Id шаблонов статусам для результата.
    /// </summary>
    /// <param name="templateStatuses">Список статусов.</param>
    Task SetProjectManagmentTemplateIdsAsync(List<ProjectManagmentTaskTemplateResult> templateStatuses);
}