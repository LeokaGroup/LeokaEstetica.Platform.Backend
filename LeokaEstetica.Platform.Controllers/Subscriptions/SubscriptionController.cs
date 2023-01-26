using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Core.Filters;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Subscriptions;

/// <summary>
/// Контроллер работы с подписками.
/// </summary>
[AuthFilter]
[ApiController]
[Route("subscriptions")]
public class SubscriptionController : BaseController
{
    /// <inheritdoc />
    public SubscriptionController()
    {
    }
}