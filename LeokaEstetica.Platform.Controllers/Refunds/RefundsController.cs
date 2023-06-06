using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Controllers.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Controllers.Refunds;

/// <summary>
/// Контроллер работы с возвратами.
/// </summary>
[AuthFilter]
[ApiController]
[Route("refunds")]
public class RefundsController : BaseController
{
    private readonly ILogger<RefundsController> _logger;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    public RefundsController(ILogger<RefundsController> logger)
    {
        _logger = logger;
    }
}