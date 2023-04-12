using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Controllers.Filters;
using LeokaEstetica.Platform.Models.Dto.Input.Notification;
using LeokaEstetica.Platform.Models.Dto.Output.Notification;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Redis.Abstractions.Notification;
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
    private readonly IProjectNotificationsService _projectNotificationsService;
    private readonly INotificationsRedisService _notificationsRedisService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectNotificationsService">Сервис уведомлений проектов.</param>
    /// <param name="notificationsRedisService">Сервис уведомлений кэша.</param>
    public NotificationsController(IProjectNotificationsService projectNotificationsService, 
        INotificationsRedisService notificationsRedisService)
    {
        _projectNotificationsService = projectNotificationsService;
        _notificationsRedisService = notificationsRedisService;
    }

    /// <summary>
    /// Метод получает список уведомлений в проекты пользователя.
    /// </summary>
    /// <returns>Список уведомлений.</returns>
    [HttpGet]
    [Route("all-notifications")]
    [ProducesResponseType(200, Type = typeof(NotificationResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<NotificationResultOutput> GetUserProjectsNotificationsAsync()
    {
        var result = await _projectNotificationsService.GetUserProjectsNotificationsAsync(GetUserName());

        return result;
    }

    /// <summary>
    /// Метод апрувит приглашение в проект.
    /// </summary>
    /// <param name="approveProjectInviteInput">Входная модель.</param>
    [HttpPatch]
    [Route("approve-project-invite")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task ApproveProjectInviteAsync([FromBody] ApproveProjectInviteInput approveProjectInviteInput)
    {
        await _projectNotificationsService.ApproveProjectInviteAsync(approveProjectInviteInput.NotificationId,
            GetUserName(), GetTokenFromHeader());
    }
    
    /// <summary>
    /// Метод реджектит приглашение в проект.
    /// </summary>
    /// <param name="rejectProjectInviteInput">Входная модель.</param>
    [HttpPatch]
    [Route("reject-project-invite")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task RejectProjectInviteAsync([FromBody] RejectProjectInviteInput rejectProjectInviteInput)
    {
        await _projectNotificationsService.RejectProjectInviteAsync(rejectProjectInviteInput.NotificationId,
            GetUserName(), GetTokenFromHeader());
    }

    /// <summary>
    /// Метод сохраняет в кэше ConnectionId.
    /// </summary>
    /// <param name="commitConnectionInput">Входная модель.</param>
    [HttpPost]
    [Route("commit-connectionid")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task AddConnectionIdCacheAsync([FromBody] CommitConnectionInput commitConnectionInput)
    {
        await _notificationsRedisService.AddConnectionIdCacheAsync(commitConnectionInput.ConnectionId,
            GetTokenFromHeader());
    }
}