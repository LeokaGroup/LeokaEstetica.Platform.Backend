using LeokaEstetica.Platform.Database.Abstractions.Press;
using LeokaEstetica.Platform.Models.Entities.Communication;
using LeokaEstetica.Platform.Services.Abstractions.Press;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Services.Services.Press;

/// <summary>
/// Класс реализует методы сервиса прессы.
/// </summary>
internal sealed class PressService : IPressService
{
    private readonly IPressRepository _pressRepository;
    private readonly ILogger<PressService> _logger;

    /// <summary>
    /// Конструктор.
    /// <param name="pressRepository">Репозиторий прессы.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="mapper">Маппер.</param>
    /// </summary>
    public PressService(IPressRepository pressRepository,
        ILogger<PressService> logger)
    {
        _pressRepository = pressRepository;
        _logger = logger;
    }

    /// <summary>
    /// Метод получает список контактов.
    /// </summary>
    /// <returns>Список контактов.</returns>
    public async Task<IEnumerable<ContactEntity>> GetContactsAsync()
    {
        try
        {
            var result = await _pressRepository.GetContactsAsync();

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}