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
    
    public LandingController(ILandingService landingService)
    {
        _landingService = landingService;
    }

    /// <summary>
    /// Метод получает данные для блока фона для главного лендоса.
    /// </summary>
    /// <returns>Данные блока.returns>
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
}