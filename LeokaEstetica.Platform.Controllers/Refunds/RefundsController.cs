using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Models.Dto.Input.Refunds;
using LeokaEstetica.Platform.Models.Dto.Output.Refunds;
using LeokaEstetica.Platform.Services.Abstractions.Refunds;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Refunds;

/// <summary>
/// Контроллер работы с возвратами.
/// </summary>
[AuthFilter]
[ApiController]
[Route("refunds")]
public class RefundsController : BaseController
{
    private readonly IRefundsService _refundsService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="refundsService">Сервис возвратов в нашей системе.</param>
    /// <param name="mapper">Автомаппер.</param>
    public RefundsController(IRefundsService refundsService, 
        IMapper mapper)
    {
        _refundsService = refundsService;
        _mapper = mapper;
    }

    /// <summary>
    /// Метод вычисляет сумму возврата заказа.
    /// Возврат делается только за неиспользованный период подписки.
    /// </summary>
    /// <returns>Выходная модель.</returns>
    [HttpGet]
    [Route("calculate")]
    [ProducesResponseType(200, Type = typeof(CalculateRefundOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<CalculateRefundOutput> CalculateRefundAsync()
    {
        var result = await _refundsService.CalculateRefundAsync(GetUserName(), CreateTokenFromHeader());

        return result;
    }

    /// <summary>
    /// Метод создает возврат по заказу.
    /// </summary>
    /// <param name="createRefundInput">Входная модель.</param>
    /// <returns>Выходная модель.</returns>
    [HttpPost]
    [Route("refunds")]
    [ProducesResponseType(200, Type = typeof(RefundOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<RefundOutput> CreateRefundAsync([FromBody] CreateRefundInput createRefundInput)
    {
        var refund = await _refundsService.CreateRefundAsync(createRefundInput.OrderId, createRefundInput.Price,
            GetUserName(), CreateTokenFromHeader());
        
        var result = _mapper.Map<RefundOutput>(refund);

        return result;
    }
}