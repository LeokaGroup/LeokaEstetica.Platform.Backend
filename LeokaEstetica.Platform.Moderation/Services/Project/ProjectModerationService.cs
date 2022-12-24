using AutoMapper;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Project;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;
using LeokaEstetica.Platform.Models.Entities.Project;
using LeokaEstetica.Platform.Moderation.Abstractions.Project;
using LeokaEstetica.Platform.Moderation.Formatters;

namespace LeokaEstetica.Platform.Moderation.Services.Project;

/// <summary>
/// Класс реализует методы сервиса модерации проектов.
/// </summary>
public sealed class ProjectModerationService : IProjectModerationService
{
    private readonly IProjectModerationRepository _projectModerationRepository;
    private readonly ILogService _logService;
    private readonly IMapper _mapper;

    public ProjectModerationService(IProjectModerationRepository projectModerationRepository,
        ILogService logService,
        IMapper mapper)
    {
        _projectModerationRepository = projectModerationRepository;
        _logService = logService;
        _mapper = mapper;
    }

    /// <summary>
    /// Метод получает список проектов для модерации.
    /// </summary>
    /// <returns>Список проектов.</returns>
    public async Task<ProjectsModerationResult> ProjectsModerationAsync()
    {
        try
        {
            var result = new ProjectsModerationResult();
            var items = await _projectModerationRepository.ProjectsModerationAsync();
            result.Projects = CreateProjectsModerationDatesBuilder.Create(items, _mapper);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает проект для просмотра/изменения.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Данные проекта.</returns>
    public async Task<UserProjectEntity> GetProjectModerationByProjectIdAsync(long projectId)
    {
        try
        {
            var result = await _projectModerationRepository.GetProjectModerationByProjectIdAsync(projectId);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex, $"Ошибка при получении проекта для модерации. ProjectId = {projectId}");
            throw;
        }
    }
}