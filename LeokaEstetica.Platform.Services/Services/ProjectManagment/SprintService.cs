using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Services.Services.ProjectManagment;

/// <summary>
/// Класс реализует методы сервиса спринтов.
/// </summary>
internal sealed class SprintService : ISprintService
{
    private readonly ILogger<SprintService> _logger;
    private readonly ISprintRepository _sprintRepository;
    
    /// <summary>
    /// Конструктор.
    /// <param name="Логгер"></param>
    /// <param name="sprintRepository">Репозиторий спринтов.</param>
    /// </summary>
    public SprintService(ILogger<SprintService> logger,
        ISprintRepository sprintRepository)
    {
        _logger = logger;
        _sprintRepository = sprintRepository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TaskSprintExtendedOutput>> GetSprintsAsync(long projectId)
    {
        try
        {
            var result = await _sprintRepository.GetSprintsAsync(projectId);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}