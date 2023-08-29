using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Controllers.Filters;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
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
    private readonly IFareRuleRepository _fareRuleRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="fareRuleService">Сервис правил тарифов.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="fareRuleRepository">Репозиторий правил тарифов.</param>
    public FareRuleController(IFareRuleService fareRuleService,
        IMapper mapper,
        IFareRuleRepository fareRuleRepository)
    {
        _fareRuleService = fareRuleService;
        _mapper = mapper;
        _fareRuleRepository = fareRuleRepository;
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
    
    /// <summary>
    /// Метод получает детали тарифа по подписке.
    /// </summary>
    /// <param name="objectId">Id объекта (тарифа).</param>
    [HttpGet]
    [Route("details")]
    [ProducesResponseType(200, Type = typeof(FareRuleOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<FareRuleOutput> GetFareRuleDetailsByObjectIdAsync([FromQuery] int objectId)
    {
        var items = await _fareRuleRepository.GetFareRuleDetailsByObjectIdAsync(objectId);
        var result = _mapper.Map<FareRuleOutput>(items);

        return result;
    }
}