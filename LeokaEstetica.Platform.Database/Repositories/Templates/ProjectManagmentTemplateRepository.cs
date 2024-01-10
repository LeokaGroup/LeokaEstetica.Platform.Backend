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

    #region Публичные методы.

    /// <inheritdoc />
    public async Task<int?> GetProjectTemplateIdAsync(long projectId)
    {
        var result = await _pgContext.UserProjects
            .Where(p => p.ProjectId == projectId)
            .Select(p => p.TemplateId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<long>> GetTemplateStatusIdsAsync(int templateId)
    {
        var result = await _pgContext.ProjectManagmentTaskStatusIntermediateTemplates
            .Where(t => t.TemplateId == templateId)
            .Select(t => t.StatusId)
            .ToListAsync();

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProjectManagmentTaskStatusTemplateEntity>> GetTaskTemplateStatusesAsync(
        IEnumerable<long> statusIds)
    {
        var result = await _pgContext.ProjectManagmentTaskStatusTemplates
            .Where(s => statusIds.Contains(s.StatusId))
            .OrderBy(o => o.Position)
            .ToListAsync();

        return result;
    }

    /// <inheritdoc />
    public async Task<ProjectManagmentTaskStatusTemplateEntity> GetProjectManagementStatusBySysNameAsync(
        string associationStatusSysName)
    {
        var result = await _pgContext.ProjectManagmentTaskStatusTemplates
            .Where(s => s.StatusSysName.Equals(associationStatusSysName))
            .Select(s => new ProjectManagmentTaskStatusTemplateEntity
            {
                StatusName = s.StatusName,
                StatusSysName = s.StatusSysName,
                Position = s.Position,
                TaskStatusId = s.TaskStatusId,
                StatusId = s.StatusId
            })
            .FirstOrDefaultAsync();

        return result;
    }

    /// <inheritdoc />
    public async Task<long> CreateProjectManagmentTaskStatusTemplateAsync(
        ProjectManagementUserStatuseTemplateEntity statusTemplateEntity)
    {
        await _pgContext.ProjectManagementUserStatuseTemplates.AddAsync(statusTemplateEntity);
        await _pgContext.SaveChangesAsync();

        return statusTemplateEntity.StatusId;
    }

    /// <inheritdoc />
    public async Task<int> GetLastPositionUserStatusTemplateAsync(long userId)
    {
        var isEmpty = await _pgContext.ProjectManagementUserStatuseTemplates.AnyAsync(x => x.UserId == userId);

        if (!isEmpty)
        {
            // Если позиций пока нету, то начнем с первой.
            return 1;
        }
        
        var result = await _pgContext.UserTaskTags
            .Where(x => x.UserId == userId)
            .Select(x => x.Position)
            .MaxAsync();

        return result;
    }

    /// <inheritdoc />
    public async Task CreateProjectManagmentTaskStatusIntermediateTemplateAsync(long statusId, int templateId)
    {
        await _pgContext.ProjectManagmentTaskStatusIntermediateTemplates.AddAsync(
            new ProjectManagmentTaskStatusIntermediateTemplateEntity
            {
                StatusId = statusId,
                TemplateId = templateId,
                IsCustomStatus = true
            });
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод получает название статуса по TaskStatusId.
    /// </summary>
    /// <param name="taskStatusId">Id статуса задачи.</param>
    /// <returns>Название статуса.</returns>
    public async Task<string> GetStatusNameByTaskStatusIdAsync(int taskStatusId)
    {
        var result = await _pgContext.ProjectManagmentTaskStatusTemplates
            .Where(s => s.TaskStatusId == taskStatusId)
            .Select(s => s.StatusName)
            .FirstOrDefaultAsync();

        return result;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}