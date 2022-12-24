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
        var prj = await _pgContext.ModerationProjects
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);

        if (prj is null)
        {
            throw new NullReferenceException($"Не удалось найти проект для модерации. ProjectId = {projectId}");
        }

        prj.ModerationStatusId = (int)ProjectModerationStatusEnum.ApproveProject;
        await _pgContext.SaveChangesAsync();

        return true;
    }
}