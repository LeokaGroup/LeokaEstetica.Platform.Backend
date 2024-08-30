using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Controllers.Validators.Commerce;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.YandexKassa;
using LeokaEstetica.Platform.Models.Dto.Output.FareRule;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
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
    private readonly IMapper _mapper;
    private readonly ILogger<CommerceController> _logger;
    private readonly ICommerceService _commerceService;
     private readonly Lazy<IDiscordService> _discordService;

     /// <summary>
     /// Конструктор.
     /// </summary>
     /// <param name="mapper">Автомаппер.</param>
     /// <param name="logger">Сервис логера.</param>
     /// <param name="commerceService">Сервис коммерции.</param>
     /// <param name="discordService">Сервис уведомлений дискорда.</param>
     public CommerceController(IMapper mapper, 
        ILogger<CommerceController> logger, 
        ICommerceService commerceService,
         Lazy<IDiscordService> discordService)
    {
        _mapper = mapper;
        _logger = logger;
        _commerceService = commerceService;
        _discordService = discordService;
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
    public async Task<ICreateOrderOutput> CreateOrderAsync([FromBody] CreateOrderPayMasterInput createOrderInput)
    {
        var validator = await new CreateOrderValidator().ValidateAsync(createOrderInput);

        if (validator.Errors.Count > 0)
        {
            // TODO: Добавить отображение ошибок фронту через сокеты.

            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException("Ошибка создания заказа. " +
                                            $"{JsonConvert.SerializeObject(createOrderInput.CreateOrderRequest)}",
                exceptions);
                
            _logger.LogCritical(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _commerceService.CreateOrderAsync(createOrderInput.PublicId, GetUserName())
            as CreateOrderYandexKassaOutput;

        return result ??
               throw new InvalidOperationException($"Ошибка при касте к типу {nameof(CreateOrderYandexKassaOutput)}");
    }

    /// <summary>
    /// Метод создает заказ в кэше или в кролике (зависит от тарифа, услуг).
    /// </summary>
    /// <param name="createOrderCacheInput">Входная модель.</param>
    /// <returns>Данные заказа, которые хранятся в кэше.</returns>
    [HttpPost]
    [Route("order/order-form/pre")]
    [ProducesResponseType(200, Type = typeof(OrderCacheOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<OrderCacheOutput> CreateOrderCacheOrRabbitMqAsync(
        [FromBody] CreateOrderCacheInput createOrderCacheInput)
    {
        var validator = await new CreateOrderCacheValidator().ValidateAsync(createOrderCacheInput);
        
        if (validator.Errors.Count > 0)
        {
            var ex = new InvalidOperationException(
                "Переданы некорректные параметры. " +
                $"CreateOrderCacheInput: {JsonConvert.SerializeObject(createOrderCacheInput)}");
                
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            _logger.LogError(ex, ex.Message);

            throw ex;
        }

        var orderFromCache = await _commerceService.CreateOrderCacheOrRabbitMqAsync(createOrderCacheInput,
            GetUserName());
        
        var result = _mapper.Map<OrderCacheOutput>(orderFromCache);

        return result;
    }

    /// <summary>
    /// TODO: Пока не будет у нас продуктов (услуг, пакетов) - в будущем будут, когда продумаем аналитику.
    /// TODO: Если задействуем, то перенести в контроллер тарифов.
    /// Метод получает услуги и сервисы заказа из кэша.
    /// </summary>
    /// <param name="publicId">Публичный код тарифа.</param>
    /// <returns>Услуги и сервисы заказа.</returns>
    // [HttpGet]
    // [Route("fare-rule/order-form/products")]
    // [ProducesResponseType(200, Type = typeof(OrderCacheOutput))]
    // [ProducesResponseType(400)]
    // [ProducesResponseType(403)]
    // [ProducesResponseType(500)]
    // [ProducesResponseType(404)]
    // public async Task<OrderCacheOutput> GetOrderProductsCacheAsync([FromQuery] Guid publicId)
    // {
    //     var orderFromCache = await _commerceService.GetOrderProductsCacheAsync(publicId, GetUserName());
    //     var result = _mapper.Map<OrderCacheOutput>(orderFromCache);
    //
    //     return result;
    // }

    /// <summary>
    /// TODO: Продумать аналитику в новой версии монетизации.
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
    /// TODO: Что он здесь делает? Это должно быть в слое доступа, там вроде есть уже такое.
    /// TODO: При оформлении заказа уже есть проверка доступа, зачем оно вообще тут?
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
        var result = await _commerceService.IsProfileEmptyAsync(GetUserName());

        return result;
    }

    /// <summary>
    /// Метод вычисляет цену тарифа исходя из параметров.
    /// </summary>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="selectedMonth">Кол-во месяцев подписки.</param>
    /// <param name="employeeCount">Кол-во сотрудников в организации.</param>
    /// <returns>Выходная модель.</returns>
    [HttpGet]
    [Route("calculate-price")]
    [ProducesResponseType(200, Type = typeof(CalculateFareRulePriceOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<CalculateFareRulePriceOutput> CalculateFareRulePriceAsync([FromQuery] Guid publicId,
        [FromQuery] int selectedMonth, [FromQuery] int employeeCount)
    {
        var result = await _commerceService.CalculateFareRulePriceAsync(publicId, selectedMonth, employeeCount,
            GetUserName());

        return result;
    }

    /// <summary>
    /// Метод вычисляет цену за публикацию вакансии в соответствии с тарифом пользователя.
    /// </summary>
    /// <returns>Выходная модель.</returns>
    [HttpGet]
    [Route("calculate-price-vacancy")]
    [ProducesResponseType(200, Type = typeof(CalculatePostVacancyPriceOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<CalculatePostVacancyPriceOutput> CalculatePricePostVacancyAsync()
    {
        var result = await _commerceService.CalculatePricePostVacancyAsync(GetUserName());

        return result;
    }
}