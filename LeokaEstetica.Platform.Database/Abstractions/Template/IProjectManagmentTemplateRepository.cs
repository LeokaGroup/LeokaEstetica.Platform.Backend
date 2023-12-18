using LeokaEstetica.Platform.Models.Entities.Template;

namespace LeokaEstetica.Platform.Database.Abstractions.Template;

/// <summary>
/// Абстракция шаблонов модуля УП.
/// </summary>
public interface IProjectManagmentTemplateRepository
{
    /// <summary>
    /// Метод получает шаблон проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Id шаблона.</returns>
    Task<int?> GetProjectTemplateIdAsync(long projectId);
    
    /// <summary>
    /// Метод получает список Id статусов, которые принадлежат шаблону.
    /// </summary>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Список Id статусов.</returns>
    Task<IEnumerable<int>> GetTemplateStatusIdsAsync(int templateId);

    /// <summary>
    /// Метод получает статусы шаблона проекта.
    /// </summary>
    /// <param name="statusIds">Список Id статусов шаблона.</param>
    /// <returns>Список статусов шаблона.</returns>
    Task<IEnumerable<ProjectManagmentTaskStatusTemplateEntity>>
        GetTaskTemplateStatusesAsync(IEnumerable<int> statusIds);
}