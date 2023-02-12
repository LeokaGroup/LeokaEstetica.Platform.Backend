using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Project;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Project;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Moderation.Project;

/// <summary>
/// Класс реализует методы репозитория модерации проектов.
/// </summary>
public sealed class ProjectModerationRepository : IProjectModerationRepository
{
    private readonly PgContext _pgContext;

    public ProjectModerationRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод получает список проектов для модерации.
    /// </summary>
    /// <returns>Список проектов.</returns>
    public async Task<IEnumerable<ModerationProjectEntity>> ProjectsModerationAsync()
    {
        var result = await _pgContext.ModerationProjects
            .Include(up => up.UserProject)
            .Where(p => p.ModerationStatus.StatusId == (int)ProjectModerationStatusEnum.ModerationProject)
            .Select(p => new ModerationProjectEntity
            {
                ModerationId = p.ModerationId,
                ProjectId = p.ProjectId,
                UserProject = new UserProjectEntity
                {
                    ProjectName = p.UserProject.ProjectName,
                    DateCreated = p.UserProject.DateCreated
                },
                DateModeration = p.DateModeration
            })
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает проект для просмотра/изменения.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Данные проекта.</returns>
    public async Task<UserProjectEntity> GetProjectModerationByProjectIdAsync(long projectId)
    {
        var result = await _pgContext.UserProjects
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);

        return result;
    }

    /// <summary>
    /// Метод одобряет проект на модерации.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Признак подиверждения проекта.</returns>
    public async Task<bool> ApproveProjectAsync(long projectId)
    {
        var isSuccessSetStatus = await SetProjectStatus(projectId, ProjectModerationStatusEnum.ApproveProject);
        
        if (!isSuccessSetStatus)
        {
            return false;
        }
        
        var project = await _pgContext.UserProjects
            .FirstOrDefaultAsync(v => v.ProjectId == projectId);

        if (project is null)
        {
            throw new InvalidOperationException($"Не удалось найти проект. ProjectId = {projectId}");
        }
        
        // Добавляем проект в каталог.
        await _pgContext.CatalogProjects.AddAsync(new CatalogProjectEntity
        {
            ProjectId = projectId
        });
        await _pgContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Метод отклоняет проект на модерации.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Признак отклонения проекта.</returns>
    public async Task<bool> RejectProjectAsync(long projectId)
    {
        var result = await SetProjectStatus(projectId, ProjectModerationStatusEnum.RejectedProject);

        return result;
    }

    /// <summary>
    /// Метод устанавливает статус проекту.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectModerationStatus">Статус.</param>
    /// <returns>Признак подиверждения проекта.</returns>
    private async Task<bool> SetProjectStatus(long projectId, ProjectModerationStatusEnum projectModerationStatus)
    {
        var prj = await _pgContext.ModerationProjects
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);

        if (prj is null)
        {
            throw new NullReferenceException($"Не удалось найти проект для модерации. ProjectId = {projectId}");
        }

        prj.ModerationStatusId = (int)projectModerationStatus;
        await _pgContext.SaveChangesAsync();

        return true;
    }
}