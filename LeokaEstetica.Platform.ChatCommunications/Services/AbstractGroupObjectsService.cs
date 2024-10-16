using Dapper;
using LeokaEstetica.Platform.Communications.Abstractions;
using LeokaEstetica.Platform.Database.Abstractions.Communications;
using LeokaEstetica.Platform.Models.Dto.Communications.Output;

namespace LeokaEstetica.Platform.Communications.Services;

/// <summary>
/// Класс реализует методы сервиса объектов группы.
/// </summary>
internal sealed class AbstractGroupObjectsService : IAbstractGroupObjectsService
{
    private readonly ILogger<AbstractGroupObjectsService> _logger;
    private readonly IAbstractGroupObjectsRepository _abstractGroupObjectsRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="abstractGroupObjectsRepository">Репозиторий объектов группы.</param>
    public AbstractGroupObjectsService(ILogger<AbstractGroupObjectsService> logger,
        IAbstractGroupObjectsRepository abstractGroupObjectsRepository)
    {
        _logger = logger;
        _abstractGroupObjectsRepository = abstractGroupObjectsRepository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<GroupObjectDialogOutput>> GetObjectDialogsAsync(long abstractScopeId, long userId)
    {
        try
        {
            var result = (await _abstractGroupObjectsRepository.GetObjectDialogsAsync(abstractScopeId, userId))
                ?.AsList();

            if (result is null || result.Count == 0)
            {
                return Enumerable.Empty<GroupObjectDialogOutput>();
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}