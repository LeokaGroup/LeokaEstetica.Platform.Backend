using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Controllers.Filters;
using LeokaEstetica.Platform.Controllers.Validators.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.FareRule;
using LeokaEstetica.Platform.Processing.Abstractions.PayMaster;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Commerce;

/// <summary>
/// Контроллер работы с коммерцией (платежной системой, платежами, чеками и т.д).
/// </summary>
[AuthFilter]
[ApiController]
[Route("commercial")]
public class CommerceController : BaseController
{
    private readonly IPayMasterService _payMasterService;
    private readonly IFareRuleRepository _fareRuleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="payMasterService">Сервис платежей через ПС PayMaster.</param>
    /// <param name="fareRuleRepository">Репозиторий правил тарифов.</param>
    /// <param name="mapper">Автомаппер.</param>
    public CommerceController(IPayMasterService payMasterService, 
        IFareRuleRepository fareRuleRepository, 
        IMapper mapper)
    {
        _payMasterService = payMasterService;
        _fareRuleRepository = fareRuleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Метод создает заказ.
    /// </summary>
    /// <param name="createOrderInput">Входная модель.</param>
    /// <returns>Данные платежа.</returns>
    [HttpPost]
    [Route("payments")]
    [ProducesResponseType(200, Type = typeof(CreateOrderOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<CreateOrderOutput> CreateOrderAsync([FromBody] CreateOrderInput createOrderInput)
    {
        var result = new CreateOrderOutput();
        var validator = await new CreateOrderValidator().ValidateAsync(createOrderInput);

        if (validator.Errors.Any())
        {
            result.Errors = validator.Errors;

            return result;
        }
        
        result = await _payMasterService.CreateOrderAsync(createOrderInput, GetUserName(), GetTokenFromHeader());

        return result;
    }

    /// <summary>
    /// Метод получает для ФЗ информацию о тарифе.
    /// </summary>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <returns>Информация о тарифе.</returns>
    [HttpGet]
    [Route("fare-rule/order-form/{publicId}/info")]
    [ProducesResponseType(200, Type = typeof(FareRuleOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<FareRuleOutput> GetFareRuleInfoAsync([FromRoute] Guid publicId)
    {
        var fareRule = await _fareRuleRepository.GetByPublicIdAsync(publicId);
        var result = _mapper.Map<FareRuleOutput>(fareRule);

        return result;
    }

    /// <summary>
    /// Метод создает заказ в кэше и хранит его 2 часа.
    /// </summary>
    /// <param name="createOrderCacheInput">Входная модель.</param>
    /// <returns>Данные заказа, которые хранятся в кэше.</returns>
    [HttpPost]
    [Route("fare-rule/order-form/products")]
    [ProducesResponseType(200, Type = typeof(CreateOrderCacheOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public Task<CreateOrderCacheOutput> CreateOrderCacheAsync(
        [FromBody] CreateOrderCacheInput createOrderCacheInput)
    {
        throw new NotImplementedException();
    }
}