using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Controllers.Filters;
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

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="refundsService">Сервис возвратов в нашей системе.</param>
    public RefundsController(IRefundsService refundsService)
    {
        _refundsService = refundsService;
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
}