using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Controllers.Validators.Commerce;
using LeokaEstetica.Platform.Core.Filters;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;
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
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="payMasterService">Зависимость сервиса платежей через ПС PayMaster.</param>
    public CommerceController(IPayMasterService payMasterService)
    {
        _payMasterService = payMasterService;
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
        
        result = await _payMasterService.CreateOrderAsync(createOrderInput, GetUserName());

        return result;
    }
}