using LeokaEstetica.Platform.Database.Abstractions.Moderation.Project;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Moderation.Abstractions.Project;

namespace LeokaEstetica.Platform.Moderation.Services.Project;

/// <summary>
/// Класс реализует методы сервиса модерации проектов.
/// </summary>
public sealed class ProjectModerationService : IProjectModerationService
{
    private readonly IProjectModerationRepository _projectModerationRepository;
    private readonly ILogService _logService;
    
    public ProjectModerationService(IProjectModerationRepository projectModerationRepository, 
        ILogService logService)
    {
        _projectModerationRepository = projectModerationRepository;
        _logService = logService;
    }

    /// <summary>
    /// Метод получает список проектов для модерации.
    /// </summary>
    /// <returns>Список проектов.</returns>
    public async Task<IEnumerable<ModerationProjectEntity>> ProjectsModerationAsync()
    {
        try
        {
            var result = await _projectModerationRepository.ProjectsModerationAsync();

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }
}