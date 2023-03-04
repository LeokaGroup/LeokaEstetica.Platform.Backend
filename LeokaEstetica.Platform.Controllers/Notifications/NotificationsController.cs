using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Controllers.Filters;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
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
    private readonly ILogService _logService;
    private readonly INotificationsRedisService _notificationsRedisService;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectNotificationsService">Сервис уведомлений проектов.</param>
    /// <param name="logService">Сервис логгера.</param>
    /// <param name="notificationsRedisService">Сервис уведомлений кэша.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    public NotificationsController(IProjectNotificationsService projectNotificationsService, 
        ILogService logService, 
        INotificationsRedisService notificationsRedisService, 
        IUserRepository userRepository)
    {
        _projectNotificationsService = projectNotificationsService;
        _logService = logService;
        _notificationsRedisService = notificationsRedisService;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Метод получает список уведомлений в проекты пользователя.
    /// </summary>
    /// <returns>Список уведомлений.</returns>
    [HttpGet]
    [Route("")]
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
        if (approveProjectInviteInput.NotificationId <= 0)
        {
            var ex = new InvalidOperationException("Id уведомления был <= 0.");
            await _logService.LogErrorAsync(ex);
            throw ex;
        }
        
        await _projectNotificationsService.ApproveProjectInviteAsync(approveProjectInviteInput.NotificationId);
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
        if (rejectProjectInviteInput.NotificationId <= 0)
        {
            var ex = new InvalidOperationException("Id уведомления был <= 0.");
            await _logService.LogErrorAsync(ex);
            throw ex;
        }
        
        await _projectNotificationsService.RejectProjectInviteAsync(rejectProjectInviteInput.NotificationId);
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
        var userId = await _userRepository.GetUserByEmailAsync(GetUserName());
        
        await _notificationsRedisService.AddConnectionIdCacheAsync(commitConnectionInput.ConnectionId, userId);
    }
}