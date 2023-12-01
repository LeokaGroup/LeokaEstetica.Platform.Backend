using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.ProjectManagment;

/// <summary>
/// Класс реализует методы репозитория управления проектами.
/// </summary>
internal sealed class ProjectManagmentRepository : IProjectManagmentRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public ProjectManagmentRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает список стратегий представления рабочего пространства.
    /// </summary>
    /// <returns>Список стратегий.</returns>
    public async Task<IEnumerable<ViewStrategyEntity>> GetViewStrategiesAsync()
    {
        var result = await _pgContext.ViewStrategies.ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает элементы верхнего меню (хидера).
    /// </summary>
    /// <returns>Список элементов.</returns>
    public async Task<IEnumerable<ProjectManagmentHeaderEntity>> GetHeaderItemsAsync()
    {
        var result = await _pgContext.ProjectManagmentHeader
            .OrderBy(o => o.Position)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список шаблонов задач, которые пользователь может выбрать перед переходом в рабочее пространство.
    /// </summary>
    /// <returns>Список шаблонов задач.</returns>
    public async Task<IEnumerable<ProjectManagmentTaskTemplateEntityResult>> GetProjectManagmentTemplatesAsync()
    {
        var result = await _pgContext.ProjectManagmentTaskTemplates
            .Include(x => x.ProjectManagmentTaskStatusTemplates.OrderBy(o => o.Position))
            .ThenInclude(x => x.ProjectManagmentTaskStatusIntermediateTemplates)
            .OrderBy(o => o.Position)
            .GroupBy(g => g.TemplateName)
            .Select(x => new ProjectManagmentTaskTemplateEntityResult
            {
                TemplateName = x.Key,
                ProjectManagmentTaskStatusTemplates = x
                    .SelectMany(y => y.ProjectManagmentTaskStatusTemplates
                        .OrderBy(o => o.Position)),
            })
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список Id статусов, которым будем проставлять Id шаблона, к которому относятся эти статусы.
    /// </summary>
    /// <param name="templateStatusIds">Список Id статусов.</param>
    /// <returns>Словарь с Id шаблонов и статусов.</returns>
    public async Task<IDictionary<int, int>> GetTemplateStatusIdsByStatusIdsAsync(IEnumerable<int> templateStatusIds)
    {
        var result = await _pgContext.ProjectManagmentTaskStatusIntermediateTemplates
            .Where(x => templateStatusIds.Contains(x.StatusId))
            .ToDictionaryAsync(k => k.StatusId, v => v.TemplateId);

        return result;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}