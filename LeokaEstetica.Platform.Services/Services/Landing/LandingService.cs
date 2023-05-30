using AutoMapper;
using LeokaEstetica.Platform.Database.Abstractions.Landing;
using LeokaEstetica.Platform.Models.Dto.Output.Landing;
using LeokaEstetica.Platform.Models.Entities.Landing;
using LeokaEstetica.Platform.Services.Abstractions.Landing;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Services.Services.Landing;

/// <summary>
/// Класс реализует методы сервиса лендингов.
/// </summary>
public class LandingService : ILandingService
{
    private readonly ILogger<LandingService> _logger;
    private readonly ILandingRepository _landingRepository;
    private readonly IMapper _mapper;

    public LandingService(ILogger<LandingService> logger,
        ILandingRepository landingRepository,
        IMapper mapper)
    {
        _logger = logger;
        _landingRepository = landingRepository;
        _mapper = mapper;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает данные для блока фона для главного лендоса.
    /// </summary>
    /// <returns>Данные блока.returns>
    public async Task<LandingStartFonOutput> LandingStartFonAsync()
    {
        try
        {
            var fon = await _landingRepository.LandingStartFonAsync();

            if (fon is null)
            {
                throw new InvalidOperationException($"Нет данных фона для главного лендинга!");
            }

            var result = _mapper.Map<LandingStartFonOutput>(fon);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает данные для фона предложений платформы.
    /// </summary>
    /// <returns>Данные для фона.</returns>
    public async Task<PlatformOfferOutput> GetPlatformItemsAsync()
    {
        try
        {
            // Получаем данные фона.
            var fon = await _landingRepository.GetPlatformDataAsync();

            if (fon is null)
            {
                throw new InvalidOperationException("Нет данных фона для предложений платформы.");
            }

            var result = _mapper.Map<PlatformOfferOutput>(fon);

            // Получаем список элементов для фона предложений платформы.
            var items = await _landingRepository.GetPlatformItemsAsync();

            var platformOfferItemsEntities = items.ToList();
            
            if (!platformOfferItemsEntities.Any())
            {
                throw new InvalidOperationException("Нет данных списка элементов для предложений платформы.");
            }

            // Наполняем список элементов в выходной модели.
            result.PlatformOfferItems = _mapper.Map<IEnumerable<PlatformOfferItemsOutput>>(items);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список таймлайнов.
    /// </summary>
    /// <returns>Список таймлайнов.</returns>
    public async Task<Dictionary<string, List<TimelineEntity>>> GetTimelinesAsync()
    {
        try
        {
            var result = await _landingRepository.GetTimelinesAsync();

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
    
    #endregion
}