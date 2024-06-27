using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Models.Dto.Input.Notification;
using LeokaEstetica.Platform.Models.Dto.Output.Notification;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Redis.Abstractions.Connection;
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
    private readonly IConnectionService _connectionService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectNotificationsService">Сервис уведомлений проектов.</param>
    /// <param name="connectionService">Сервис уведомлений Redis.</param>
    public NotificationsController(IProjectNotificationsService projectNotificationsService, 
        IConnectionService connectionService)
    {
        _projectNotificationsService = projectNotificationsService;
        _connectionService = connectionService;
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
        await _connectionService.AddConnectionIdCacheAsync(commitConnectionInput.ConnectionId,
            GetTokenFromHeader());
    }
}