using LeokaEstetica.Platform.Access.Abstractions.User;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Messaging.Abstractions.Project;
using LeokaEstetica.Platform.Models.Entities.Communication;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;

namespace LeokaEstetica.Platform.Messaging.Services.Project;

/// <summary>
/// Класс реализует методы сервиса комментариев к проектам.
/// </summary>
public sealed class ProjectCommentsService : IProjectCommentsService
{
    private readonly ILogService _logService;
    private readonly IUserRepository _userRepository;
    private readonly IProjectCommentsRepository _projectCommentsRepository;
    private readonly IAccessUserService _accessUserService;
    private readonly IAccessUserNotificationsService _accessUserNotificationsService;
    private readonly ICommentNotificationsService _commentNotificationsService;

    public ProjectCommentsService(ILogService logService, 
        IUserRepository userRepository, 
        IProjectCommentsRepository projectCommentsRepository, 
        IAccessUserService accessUserService, 
        IAccessUserNotificationsService accessUserNotificationsService,
        ICommentNotificationsService commentNotificationsService)
    {
        _logService = logService;
        _userRepository = userRepository;
        _projectCommentsRepository = projectCommentsRepository;
        _accessUserService = accessUserService;
        _accessUserNotificationsService = accessUserNotificationsService;
        _commentNotificationsService = commentNotificationsService;
    }

    /// <summary>
    /// Метод создает комментарий к проекту.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="comment">Текст комментария.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task CreateProjectCommentAsync(long projectId, string comment, string account, string token)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                await _logService.LogErrorAsync(ex);
                throw ex;
            }
            
            // Проверяем заполнение анкеты и даем доступ либо нет.
            var isEmptyProfile = await _accessUserService.IsProfileEmptyAsync(userId);

            // Если нет доступа, то не даем оплатить платный тариф.
            if (isEmptyProfile)
            {
                var ex = new InvalidOperationException($"Анкета пользователя не заполнена. UserId был: {userId}");

                await _accessUserNotificationsService.SendNotificationWarningEmptyUserProfileAsync("Внимание",
                    "Для оставления комментария к проекту должна быть заполнена информация вашей анкеты.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);
                
                throw ex;
            }

            // Проверяем наличие комментария
            if (string.IsNullOrEmpty(comment))
            {
                var ex = new InvalidOperationException($"Комментарий к проекту не может быть пустым." +
                    $"ProjectId: {projectId}");

                await _commentNotificationsService.SendNotificationCommentProjectIsNotEmptyAsync("Внимание",
                    "Комментарий к проекту не может быть пустым.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);

                throw ex;
            }
            
            await _projectCommentsRepository.CreateProjectCommentAsync(projectId, comment, userId);
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex,
                "Ошибка при создании комментария к проекту. " +
                $"ProjectId = {projectId}. " +
                $"Comment = {comment}. Account = {account}");
            throw;
        }
    }

    /// <summary>
    /// Метод получает список комментариев проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список комментариев проекта.</returns>
    public async Task<IEnumerable<ProjectCommentEntity>> GetProjectCommentsAsync(long projectId)
    {
        try
        {
            var result = await _projectCommentsRepository.GetProjectCommentsAsync(projectId);

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }
}