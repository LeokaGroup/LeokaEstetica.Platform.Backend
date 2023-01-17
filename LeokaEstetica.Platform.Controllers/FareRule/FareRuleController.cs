using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Core.Filters;
using LeokaEstetica.Platform.Models.Dto.Output.FareRule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.FareRule;

/// <summary>
/// Контроллер правил тарифов, оферты и других правил платформы.
/// </summary>
[AuthFilter]
[ApiController]
[Route("rules")]
public class FareRuleController : BaseController
{
    public FareRuleController()
    {
    }

    /// <summary>
    /// Метод получает список тарифов.
    /// </summary>
    /// <returns>Список тарифов.</returns>
    [AllowAnonymous]
    [HttpGet]
    [Route("")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<FareRuleOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<FareRuleOutput>> GetFareRulesAsync()
    {
        
    }
}