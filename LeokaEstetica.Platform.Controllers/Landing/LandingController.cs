using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Models.Dto.Output.Landing;
using LeokaEstetica.Platform.Services.Abstractions.Landing;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Landing;

/// <summary>
/// Контроллер лендингов. Здесь находится логика главного лендинга и других тоже.
/// </summary>
[ApiController]
[Route("landing")]
public class LandingController : BaseController
{
    private readonly ILandingService _landingService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="landingService">Сервис лендинга.</param>
    /// <param name="mapper">Автомаппер.</param>
    public LandingController(ILandingService landingService, 
        IMapper mapper)
    {
        _landingService = landingService;
        _mapper = mapper;
    }

    /// <summary>
    /// Метод получает данные для блока фона для главного лендоса.
    /// </summary>
    /// <returns>Данные блока.returns></returns>
    [HttpGet]
    [Route("fon/start")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<LandingStartFonOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<LandingStartFonOutput> LandingStartFonAsync()
    {
        var result = await _landingService.LandingStartFonAsync();

        return result;
    }

    /// <summary>
    /// Метод получает данные предложений платформы.
    /// </summary>
    /// <returns>Данные предложений платформы.</returns>
    [HttpGet]
    [Route("offers")]
    [ProducesResponseType(200, Type = typeof(PlatformOfferOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<PlatformOfferOutput> GetPlatformOffersAsync()
    {
        var result = await _landingService.GetPlatformItemsAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список таймлайнов.
    /// </summary>
    /// <returns>Список таймлайнов.</returns>
    [HttpGet]
    [Route("timelines")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<TimelineOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<Dictionary<string, List<TimelineOutput>>> GetTimelinesAsync()
    {
        Dictionary<string, List<TimelineOutput>> result = null;
        
        var items = await _landingService.GetTimelinesAsync();
        
        if (items.Any())
        {
            result = _mapper.Map<Dictionary<string, List<TimelineOutput>>>(items);
        }

        return result;
    }
}