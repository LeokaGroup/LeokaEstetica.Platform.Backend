using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Controllers.Filters;
using LeokaEstetica.Platform.Models.Dto.Output.FareRule;
using LeokaEstetica.Platform.Services.Abstractions.FareRule;
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
    private readonly IFareRuleService _fareRuleService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="fareRuleService">Сервис правил тарифов.</param>
    /// <param name="mapper">Автомаппер.</param>
    public FareRuleController(IFareRuleService fareRuleService,
        IMapper mapper)
    {
        _fareRuleService = fareRuleService;
        _mapper = mapper;
    }

    /// <summary>
    /// Метод получает список тарифов.
    /// </summary>
    /// <returns>Список тарифов.</returns>
    [HttpGet]
    [Route("get-rules")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<FareRuleOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<FareRuleOutput>> GetFareRulesAsync()
    {
        var items = await _fareRuleService.GetFareRulesAsync();
        var result = _mapper.Map<IEnumerable<FareRuleOutput>>(items);

        return result;
    }
}