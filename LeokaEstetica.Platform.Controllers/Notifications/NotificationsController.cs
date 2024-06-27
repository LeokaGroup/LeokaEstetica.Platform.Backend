using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Input.Notification;
using LeokaEstetica.Platform.Models.Dto.Output.Notification;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Redis.Abstractions.Connection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<NotificationsController> _logger;
    private readonly Lazy<IDiscordService> _discordService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectNotificationsService">Сервис уведомлений проектов.</param>
    /// <param name="connectionService">Сервис уведомлений Redis.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    public NotificationsController(IProjectNotificationsService projectNotificationsService, 
        IConnectionService connectionService,
         ILogger<NotificationsController> logger,
          Lazy<IDiscordService> discordService)
    {
        _projectNotificationsService = projectNotificationsService;
        _connectionService = connectionService;
        _logger = logger;
        _discordService = discordService;
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

    /// <summary>
    /// Метод получает список приглашений в проект.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список приглашений в проект.</returns>
    [HttpGet]
    [Route("project-invites")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectInviteOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProjectInviteOutput>> GetProjectInvitesAsync([FromQuery] long projectId)
    {
        if (projectId <= 0)
        {
            var ex = new AggregateException("Ошибка при получении приглашений проекта. " +
                                            $"ProjectId: {projectId}.");
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _projectNotificationsService.GetProjectInvitesAsync(projectId);

        return result;
    }
}