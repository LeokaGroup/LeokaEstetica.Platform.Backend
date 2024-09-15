using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Services.Helpers;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Services.Services.ProjectManagment;

/// <summary>
/// Класс реализует методы сервиса эпика.
/// </summary>
internal sealed class EpicService : IEpicService
{
    private readonly IEpicRepository _epicRepository;
    private readonly ILogger<EpicService> _logger;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="epicRepository"></param>
    /// <param name="logger">Логгер.</param>
    public EpicService(IEpicRepository epicRepository,
        ILogger<EpicService> logger)
    {
        _epicRepository = epicRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task ExcludeEpicTasksAsync(long epicId, IEnumerable<string>? projectTaskIds)
    {
        try
        {
            await _epicRepository.ExcludeEpicTasksAsync(epicId,
                projectTaskIds!.Select(x => x.GetProjectTaskIdFromPrefixLink()));
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }
}