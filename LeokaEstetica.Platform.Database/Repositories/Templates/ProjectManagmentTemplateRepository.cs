using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Template;
using LeokaEstetica.Platform.Models.Entities.Template;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Templates;

/// <summary>
/// Класс реализует методы репозитория шаблонов модуля УП.
/// </summary>
internal sealed class ProjectManagmentTemplateRepository : IProjectManagmentTemplateRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструктор.
    /// <param name="pgContext">Датаконтекст.</param>
    /// </summary>
    public ProjectManagmentTemplateRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод получает шаблон проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Id шаблона.</returns>
    public async Task<int?> GetProjectTemplateIdAsync(long projectId)
    {
        var result = await _pgContext.UserProjects
            .Where(p => p.TemplateId.Value == projectId)
            .Select(p => p.TemplateId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список Id статусов, которые принадлежат шаблону.
    /// </summary>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Список Id статусов.</returns>
    public async Task<IEnumerable<int>> GetTemplateStatusIdsAsync(int templateId)
    {
        var result = await _pgContext.ProjectManagmentTaskStatusIntermediateTemplates
            .Where(t => t.TemplateId == templateId)
            .Select(t => t.StatusId)
            .ToListAsync();

        return result;
    }

    /// <summary>
        /// Метод получает статусы шаблона проекта.
        /// </summary>
        /// <param name="statusIds">Список Id статусов шаблона.</param>
        /// <returns>Список статусов шаблона.</returns>
    public async Task<IEnumerable<ProjectManagmentTaskStatusTemplateEntity>> GetTaskTemplateStatusesAsync(
        IEnumerable<int> statusIds)
    {
        var result = await _pgContext.ProjectManagmentTaskStatusTemplates
            .Where(s => statusIds.Contains(s.StatusId))
            .ToListAsync();

        return result;
    }
}