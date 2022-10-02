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
    public async Task<LandingStartFonOutput> LandingStartFonAsync()
    {
        var result = await _landingService.LandingStartFonAsync();

        return result;
    }
}