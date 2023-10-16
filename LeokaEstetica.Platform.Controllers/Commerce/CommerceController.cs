using AutoMapper;
using FluentValidation.Results;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Controllers.Filters;
using LeokaEstetica.Platform.Controllers.Validators.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.FareRule;
using LeokaEstetica.Platform.Processing.Abstractions.Commerce;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Controllers.Commerce;

/// <summary>
/// Контроллер работы с коммерцией (платежной системой, платежами, чеками и т.д).
/// </summary>
[AuthFilter]
[ApiController]
[Route("commercial")]
public class CommerceController : BaseController
{
    private readonly IFareRuleRepository _fareRuleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CommerceController> _logger;
    private readonly ICommerceService _commerceService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="fareRuleRepository">Репозиторий правил тарифов.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="logger">Сервис логера.</param>
    /// <param name="commerceService">Сервис коммерции.</param>
    public CommerceController(IFareRuleRepository fareRuleRepository, 
        IMapper mapper, 
        ILogger<CommerceController> logger, 
        ICommerceService commerceService)
    {
        _fareRuleRepository = fareRuleRepository;
        _mapper = mapper;
        _logger = logger;
        _commerceService = commerceService;
    }

    /// <summary>
    /// Метод создает заказ.
    /// </summary>
    /// <param name="createOrderInput">Входная модель.</param>
    /// <returns>Данные платежа.</returns>
    [HttpPost]
    [Route("payments")]
    [ProducesResponseType(200, Type = typeof(ICreateOrderOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ICreateOrderOutput> CreateOrderAsync(
        [FromBody] CreateOrderPayMasterInput createOrderInput)
    {
        var result = new CreateOrderPayMasterOutput { Errors = new List<ValidationFailure>() };
        var validator = await new CreateOrderValidator().ValidateAsync(createOrderInput);

        if (validator.Errors.Any())
        {
            result.Errors = validator.Errors;

            return result;
        }

        result = await _commerceService.CreateOrderAsync(createOrderInput.PublicId, GetUserName(),
            GetTokenFromHeader()) as CreateOrderPayMasterOutput;

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
    [Route("fare-rule/order-form/pre")]
    [ProducesResponseType(200, Type = typeof(OrderCacheOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<OrderCacheOutput> CreateOrderCacheAsync(
        [FromBody] CreateOrderCacheInput createOrderCacheInput)
    {
        var validator = await new CreateOrderCacheValidator().ValidateAsync(createOrderCacheInput);
        
        if (validator.Errors.Any())
        {
            var ex = new InvalidOperationException(
                "Переданы некорректные параметры. " +
                $"CreateOrderCacheInput: {JsonConvert.SerializeObject(createOrderCacheInput)}");
            _logger.LogError(ex, ex.Message);

            throw ex;
        }

        var orderFromCache = await _commerceService.CreateOrderCacheAsync(createOrderCacheInput, GetUserName());
        var result = _mapper.Map<OrderCacheOutput>(orderFromCache);

        return result;
    }

    /// <summary>
    /// Метод получает услуги и сервисы заказа из кэша.
    /// </summary>
    /// <param name="publicId">Публичный код тарифа.</param>
    /// <returns>Услуги и сервисы заказа.</returns>
    [HttpGet]
    [Route("fare-rule/order-form/products")]
    [ProducesResponseType(200, Type = typeof(OrderCacheOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<OrderCacheOutput> GetOrderProductsCacheAsync([FromQuery] Guid publicId)
    {
        var orderFromCache = await _commerceService.GetOrderProductsCacheAsync(publicId, GetUserName());
        var result = _mapper.Map<OrderCacheOutput>(orderFromCache);

        return result;
    }

    /// <summary>
    /// Метод вычисляет, есть ли остаток с прошлой подписки пользователя для учета ее как скидку при оформлении новой подписки.
    /// </summary>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="month">Кол-во месяцев подписки.</param>
    /// <returns>Сумма остатка, если она есть.</returns>
    [HttpGet]
    [Route("check-free")]
    [ProducesResponseType(200, Type = typeof(OrderFreeOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<OrderFreeOutput> CheckFreePriceAsync([FromQuery] Guid publicId, [FromQuery] short month)
    {
        var result = await _commerceService.CheckFreePriceAsync(GetUserName(), publicId, month);

        return result;
    }

    /// <summary>
    /// Метод проверяет заполнение анкеты пользователя.
    /// Если не заполнена, то нельзя оформить заказ.
    /// </summary>
    /// <returns>Признак результата проверки. False - Анкета заполнена. True - не заполнена.</returns>
    [HttpGet]
    [Route("check-profile")]
    [ProducesResponseType(200, Type = typeof(bool))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<bool> IsProfileEmptyAsync()
    {
        var result = await _commerceService.IsProfileEmptyAsync(GetUserName(), GetTokenFromHeader());

        return result;
    }
}