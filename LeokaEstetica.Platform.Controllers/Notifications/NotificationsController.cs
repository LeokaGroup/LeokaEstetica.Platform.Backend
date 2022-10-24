using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Core.Filters;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Models.Input;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Notifications;

/// <summary>
/// Контроллер работы с уведомлениями.
/// </summary>
[AuthFilter]
[ApiController]
[Route("notifications")]
public class NotificationsController : BaseController
{
    private readonly INotificationsService _notificationsService;
    
    public NotificationsController(INotificationsService notificationsService)
    {
        _notificationsService = notificationsService;
    }

    /// <summary>
    /// Метод записывает Id подключения, который создается при подключении SignalR в кэш.
    /// </summary>
    /// <param name="connectionInput">Входная модель.</param>
    [HttpPost]
    [Route("signalr-connectionid")]
    public async Task SaveConnectionIdCacheAsync([FromBody] ConnectionInput connectionInput)
    {
        await _notificationsService.SaveConnectionIdCacheAsync(connectionInput.ConnectionId, connectionInput.UserCode);
    }
}