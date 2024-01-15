using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using LeokaEstetica.Platform.Models.Entities.Template;
using Microsoft.EntityFrameworkCore;
using SqlKata.Execution;

namespace LeokaEstetica.Platform.Database.Repositories.ProjectManagment;

/// <summary>
/// Класс реализует методы репозитория управления проектами.
/// </summary>
internal sealed class ProjectManagmentRepository : BaseRepository, IProjectManagmentRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public ProjectManagmentRepository(PgContext pgContext, IConnectionProvider connectionProvider)
        : base(connectionProvider)
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
        // var result = await _pgContext.ViewStrategies
        //     .OrderBy(o => o.Position)
        //     .ToListAsync();

        using var connection = await ConnectionProvider.GetConnectionAsync();
        var queryFactory = await ConnectionProvider.CreateQueryFactory(connection);
        var query = queryFactory.Query("Users").Where("Id", 1).Where("Status", "Active");

        return null;
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
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Список шаблонов задач.</returns>
    public async Task<IEnumerable<ProjectManagmentTaskTemplateEntityResult>> GetProjectManagmentTemplatesAsync(
        long? templateId)
    {
        var projectManagmentTaskTemplates = _pgContext.ProjectManagmentTaskTemplates.AsQueryable();

        if (templateId.HasValue)
        {
            projectManagmentTaskTemplates = projectManagmentTaskTemplates
                .Where(x => x.TemplateId == templateId.Value)
                .OrderBy(o => o.Position);
        }
        
        var result = await projectManagmentTaskTemplates
            .Include(x => x.ProjectManagmentTaskStatusTemplates.OrderBy(o => o.Position))
            .ThenInclude(x => x.ProjectManagmentTaskStatusIntermediateTemplates)
            .OrderBy(o => o.Position)
            .GroupBy(g => g.TemplateName)
            .Select(x => new ProjectManagmentTaskTemplateEntityResult
            {
                TemplateName = x.Key,
                ProjectManagmentTaskStatusTemplates = x
                    .SelectMany(y => y.ProjectManagmentTaskStatusTemplates
                        .OrderBy(o => o.Position))
            })
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список Id статусов, которым будем проставлять Id шаблона, к которому относятся эти статусы.
    /// </summary>
    /// <param name="templateStatusIds">Список Id статусов.</param>
    /// <returns>Словарь с Id шаблонов и статусов.</returns>
    public async Task<IEnumerable<KeyValuePair<long, int>>> GetTemplateStatusIdsByStatusIdsAsync(
        IEnumerable<long> templateStatusIds)
    {
        var result = await _pgContext.ProjectManagmentTaskStatusIntermediateTemplates
            .Where(x => templateStatusIds.Contains(x.StatusId))
            .OrderBy(o => o.TemplateId)
            .Select(x => new KeyValuePair<long, int>(x.StatusId, x.TemplateId))
            .ToListAsync();

        return result;
    }
    
    /// <summary>
    /// Метод получает задачи проекта для рабочего пространства.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Задачи проекта.</returns>
    public async Task<IEnumerable<ProjectTaskEntity>> GetProjectTasksAsync(long projectId)
    {
        var result = await _pgContext.ProjectTasks
            .Where(t => t.ProjectId == projectId)
            .OrderBy(o => o.ProjectId)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает названия тегов (меток) пользователя задач по их Id.
    /// </summary>
    /// <param name="tagIds">Id тегов (меток) задач.</param>
    /// <returns>Словарь с тегами (метками) задач.</returns>
    public async Task<IDictionary<int, string>> GetTagNamesByTagIdsAsync(IEnumerable<int> tagIds)
    {
        var result = await _pgContext.UserTaskTags
            .Where(t => tagIds.Contains(t.TagId))
            .OrderBy(o => o.Position)
            .ToDictionaryAsync(k => k.TagId, v => v.TagName);

        return result;
    }

    /// <summary>
    /// Метод получает названия типов задач по их Id.
    /// </summary>
    /// <param name="typeIds">Id типов задач.</param>
    /// <returns>Словарь с типами задач.</returns>
    public async Task<IDictionary<int, string>> GetTypeNamesByTypeIdsAsync(IEnumerable<int> typeIds)
    {
        var result = await _pgContext.TaskTypes
            .Where(t => typeIds.Contains(t.TypeId))
            .OrderBy(o => o.Position)
            .ToDictionaryAsync(k => k.TypeId, v => v.TypeName);

        return result;
    }

    /// <summary>
    /// Метод получает названия резолюций задач по их Id.
    /// </summary>
    /// <param name="resolutionIds">Id резолюций задач.</param>
    /// <returns>Словарь с резолюциями задач.</returns>
    public async Task<IDictionary<int, string>> GetResolutionNamesByResolutionIdsAsync(IEnumerable<int> resolutionIds)
    {
        var result = await _pgContext.TaskResolutions
            .Where(t => resolutionIds.Contains(t.ResolutionId))
            .OrderBy(o => o.Position)
            .ToDictionaryAsync(k => k.ResolutionId, v => v.ResolutionName);

        return result;
    }

    /// <summary>
    /// Метод получает детали задачи.
    /// </summary>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Данные задачи.</returns>
    public async Task<ProjectTaskEntity> GetTaskDetailsByTaskIdAsync(long projectTaskId, long projectId)
    {
        var result = await _pgContext.ProjectTasks
            .FirstOrDefaultAsync(t => t.ProjectId == projectId 
                                      && t.ProjectTaskId == projectTaskId);

        return result;
    }
    
    /// <summary>
    /// Метод получает названия приоритетов задач по их Id.
    /// </summary>
    /// <param name="priorityIds">Id приоритетов задач.</param>
    /// <returns>Словарь с приоритетами задач.</returns>
    public async Task<IDictionary<int, string>> GetPriorityNamesByPriorityIdsAsync(IEnumerable<int> priorityIds)
    {
        var result = await _pgContext.TaskPriorities
            .Where(t => priorityIds.Contains(t.PriorityId))
            .OrderBy(o => o.Position)
            .ToDictionaryAsync(k => k.PriorityId, v => v.PriorityName);

        return result;
    }

    /// <summary>
    /// Метод получает последний Id задачи в рамках проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Последний Id задачи в рамках проекта.</returns>
    public async Task<long> GetLastProjectTaskIdAsync(long projectId)
    {
        var result = await _pgContext.ProjectTasks
            .Where(t => t.ProjectId == projectId)
            .MaxAsync(t => t.ProjectTaskId);

        return result;
    }

    /// <summary>
    /// Метод получает список приоритетов задачи.
    /// </summary>
    /// <returns>Список приоритетов задачи.</returns>
    public async Task<IEnumerable<TaskPriorityEntity>> GetTaskPrioritiesAsync()
    {
        var result = await _pgContext.TaskPriorities
            .OrderBy(o => o.Position)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список типов задач.
    /// </summary>
    /// <returns>Список типов задач.</returns>
    public async Task<IEnumerable<TaskTypeEntity>> GetTaskTypesAsync()
    {
        var result = await _pgContext.TaskTypes
            .OrderBy(o => o.Position)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список тегов пользователя для выбора в задаче.
    /// </summary>
    /// <returns>Список тегов.</returns>
    public async Task<IEnumerable<UserTaskTagEntity>> GetTaskTagsAsync()
    {
        var result = await _pgContext.UserTaskTags
            .OrderBy(o => o.Position)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод создает задачу проекта.
    /// </summary>
    /// <param name="task">Задача для создания.</param>
    public async Task CreateProjectTaskAsync(ProjectTaskEntity task)
    {
        await _pgContext.ProjectTasks.AddAsync(task);
        await _pgContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<int> GetLastPositionUserTaskTagAsync(long userId)
    {
        var isEmpty = await _pgContext.UserTaskTags.AnyAsync(x => x.UserId == userId);

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
    public async Task CreateUserTaskTagAsync(UserTaskTagEntity tag)
    {
        await _pgContext.UserTaskTags.AddAsync(tag);
        await _pgContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<bool> IfProjectHavingProjectTaskIdAsync(long projectId, long projectTaskId)
    {
        var result = await _pgContext.ProjectTasks
            .Where(t => t.ProjectId == projectId
                        && t.ProjectTaskId == projectTaskId)
            .AnyAsync();

        return result;
    }

    /// <inheritdoc />
    public async Task<long> GetProjectTaskStatusIdByProjectIdProjectTaskIdAsync(long projectId, long projectTaskId)
    {
        var result = await _pgContext.ProjectTasks
            .Where(t => t.ProjectId == projectId
                        && t.ProjectTaskId == projectTaskId)
            .Select(t => t.TaskStatusId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<long>> GetProjectManagementTransitionIntermediateTemplatesAsync(
            long currentTaskStatusId)
    {
        var result = await _pgContext.ProjectManagementTransitionIntermediateTemplates
            .Where(t => t.FromStatusId == currentTaskStatusId)
            .Select(t => t.ToStatusId)
            .ToListAsync();

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProjectManagmentTaskStatusIntermediateTemplateEntity>>
        GetTaskStatusIntermediateTemplatesAsync(IEnumerable<long> statusIds)
    {
        var result = await _pgContext.ProjectManagmentTaskStatusIntermediateTemplates
            .Where(s => statusIds.Contains(s.StatusId))
            .ToListAsync();

        return result;
    }

    /// <inheritdoc />
    public async Task<IDictionary<long, ProjectManagmentTaskStatusTemplateEntity>> GetTaskStatusTemplatesAsync()
    {
        var result = await _pgContext.ProjectManagmentTaskStatusTemplates
            .ToDictionaryAsync(k => k.StatusId, v => v);

        return result;
    }

    /// <inheritdoc />
    public async Task<IDictionary<long, ProjectManagementUserStatuseTemplateEntity>> GetUserTaskStatusTemplatesAsync()
    {
        var result = await _pgContext.ProjectManagementUserStatuseTemplates
            .ToDictionaryAsync(k => k.StatusId, v => v);

        return result;
    }

    /// <inheritdoc />
    public async Task ChangeTaskStatusAsync(long projectId, long changeStatusId, long taskId)
    {
        var task = await _pgContext.ProjectTasks
            .FirstOrDefaultAsync(t => t.ProjectId == projectId 
                                      && t.TaskId == taskId);

        if (task is null)
        {
            throw new InvalidOperationException($"Задача с Id: {taskId} не найдена у проекта: {projectId}.");
        }

        task.TaskStatusId = changeStatusId;
        await _pgContext.SaveChangesAsync();
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}