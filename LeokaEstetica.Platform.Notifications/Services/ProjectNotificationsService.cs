using System.Runtime.CompilerServices;
using AutoMapper;
using LeokaEstetica.Platform.Base.Abstractions.Messaging.Mail;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.Notification;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Notification;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Notifications.Data;
using LeokaEstetica.Platform.Redis.Abstractions.Connection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NotificationOutput = LeokaEstetica.Platform.Notifications.Models.Output.NotificationOutput;
using NotificationProjectOutput = LeokaEstetica.Platform.Models.Dto.Output.Notification.NotificationOutput;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Notifications.Services;

/// <summary>
/// Класс реализует методы уведомлений проектов.
/// </summary>
internal sealed class ProjectNotificationsService : IProjectNotificationsService
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ILogger<ProjectNotificationsService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IProjectNotificationsRepository _projectNotificationsRepository;
    private readonly IMapper _mapper;
    private readonly IConnectionService _connectionService;
    private readonly IProjectRepository _projectRepository;
    private readonly IMailingsService _mailingsService;
    private readonly IGlobalConfigRepository _globalConfigRepository;
    private readonly IVacancyRepository _vacancyRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="hubContext">Контекст хаба.</param>
    /// <param name="logService">Сервис логера.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="projectNotificationsRepository">Репозиторий уведомлений проектов.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="connectionService">Сервис подключений Redis.</param>
    public ProjectNotificationsService(IHubContext<ChatHub> hubContext, 
        ILogger<ProjectNotificationsService> logger, 
        IUserRepository userRepository,
        IMapper mapper, 
        IProjectNotificationsRepository projectNotificationsRepository, 
        IConnectionService connectionService, 
        IProjectRepository projectRepository, 
        IMailingsService mailingsService, 
        IGlobalConfigRepository globalConfigRepository, 
        IVacancyRepository vacancyRepository)
    {
        _hubContext = hubContext;
        _logger = logger;
        _userRepository = userRepository;
        _mapper = mapper;
        _projectNotificationsRepository = projectNotificationsRepository;
        _connectionService = connectionService;
        _projectRepository = projectRepository;
        _mailingsService = mailingsService;
        _globalConfigRepository = globalConfigRepository;
        _vacancyRepository = vacancyRepository;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод отправляет уведомление об успешном создании проекта пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationSuccessCreatedUserProjectAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationSuccessCreatedUserProject", new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }

    /// <summary>
    /// Метод отправляет уведомление об ошибке при создании проекта пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationErrorCreatedUserProjectAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationErrorCreatedUserProject", new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }

    /// <summary>
    /// Метод отправляет уведомление о дубликате проекта пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationWarningDublicateUserProjectAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationWarningDublicateUserProject", new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }

    /// <summary>
    /// Метод отправляет уведомление об успехе при изменении проекта пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationSuccessUpdatedUserProjectAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationSuccessUpdatedUserProject", new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }

    /// <summary>
    /// Метод отправляет уведомление об ошибке при изменении проекта пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationErrorUpdatedUserProjectAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationErrorUpdatedUserProject", new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }

    /// <summary>
    /// Метод отправляет уведомление об успешной привязке вакансии к проекту.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationSuccessAttachProjectVacancyAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationSuccessAttachProjectVacancy", new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }

    /// <summary>
    /// Метод отправляет уведомление об дубликате при привязке вакансии к проекту.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationErrorDublicateAttachProjectVacancyAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);
        
        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationErrorDublicateAttachProjectVacancy",
            new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }

    /// <summary>
    /// Метод отправляет уведомление об отклике на проект.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationSuccessProjectResponseAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationSuccessProjectResponse",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление предупреждения об отклике на проект.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationWarningProjectResponseAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationWarningProjectResponse",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление предупреждения о не найденных пользователях по поисковому запросу.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationWarningSearchProjectTeamMemberAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationWarningSearchProjectTeamMember",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление предупреждения об ошибке при добавлении пользователей в команду проекта.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationWarningInviteProjectTeamMembersAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationWarningInviteProjectTeamMembers",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление ошибки об ошибке при добавлении пользователей в команду проекта.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationErrorInviteProjectTeamMembersAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationErrorInviteProjectTeamMembers",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление об предупреждении лимите проектов по тарифу.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationWarningLimitFareRuleProjectsAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationWarningLimitFareRuleProjects",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление об ошибке при удалении вакансии проекта.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationErrorDeleteProjectVacancyAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationErrorDeleteProjectVacancy",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление об успехе при удалении вакансии проекта.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationSuccessDeleteProjectVacancyAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationSuccessDeleteProjectVacancy",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление об ошибке при удалении проекта.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationErrorDeleteProjectAsync(string title, string notifyText, 
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationErrorDeleteProject",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление об успехе при удалении проекта.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationSuccessDeleteProjectAsync(string title, string notifyText,
        string notificationLevel, string userId)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(userId);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationSuccessDeleteProject",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление об ошибке при отклике на проект.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationErrorProjectResponseAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationErrorProjectResponse",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }
    
    /// <summary>
    /// Метод получает список уведомлений в проекты пользователя.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список уведомлений.</returns>
    public async Task<NotificationResultOutput> GetUserProjectsNotificationsAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            // Получаем список уведомлений инвайтов в проект пользователя.
            var items = await _projectNotificationsRepository.GetUserProjectsNotificationsAsync(userId);

            var notificationsCount = items.UserNotifications.Count + items.OwnerNotifications.Count;
            var result = new NotificationResultOutput
            {
                Notifications = new List<NotificationProjectOutput>(notificationsCount)
            };

            if (items.UserNotifications.Any())
            {
                result.Notifications = _mapper.Map<List<NotificationProjectOutput>>(items.UserNotifications);
            }
            
            // Работаем с уведомлениями владельцев.
            if (items.OwnerNotifications.Any())
            {
                var newOwnerNotifications = _mapper.Map<List<NotificationProjectOutput>>(items.OwnerNotifications);
                result.Notifications.AddRange(newOwnerNotifications);
            }

            var resultNotifications = result.Notifications;

            // Проставляем признак отображения кнопок.
            foreach (var notification in resultNotifications)
            {
                var isProjectOwner = await _projectRepository.CheckProjectOwnerAsync(notification.ProjectId, userId);

                // Если страницу уведомлений просматривает владелец.
                if (isProjectOwner)
                {
                    // Если инициатором приглашения был владелец, то не отображать кнопку апрува у него.
                    if (notification.UserId != userId)
                    {
                        notification.IsAcceptButton = false;
                    }
                    
                    else
                    {
                        notification.IsAcceptButton = true;
                    }
                    
                    notification.IsRejectButton = true;
                    notification.IsVisibleNotificationsButtons = true;
                }

                // Иначе скрываем кнопки.
                else
                {
                    // Если инициатором приглашения был не владелец, то отображать кнопку апрува у него.
                    if (notification.UserId == userId)
                    {
                        notification.IsVisibleNotificationsButtons = true;
                        notification.IsAcceptButton = true;
                        notification.IsRejectButton = true;
                    }
                    
                    else
                    {
                        notification.IsVisibleNotificationsButtons = false;
                        notification.IsAcceptButton = false;
                        notification.IsRejectButton = false;
                    }
                }
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод апрувит приглашение в проект.
    /// </summary>
    /// <param name="notificationId">Id уведомления.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task ApproveProjectInviteAsync(long notificationId, string account, string token)
    {
        try
        {
            if (notificationId <= 0)
            {
                var ex = new InvalidOperationException("Id уведомления был <= 0.");
                throw ex;
            }
            
            // Проверяем существование уведомления.
            var isExistsNotification = await _projectNotificationsRepository
                .CheckExistsNotificationByIdAsync(notificationId);
            
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            if (!isExistsNotification)
            {
                var ex = new InvalidOperationException(
                    $"Уведомления с NotificationId: {notificationId} не существует.");

                await SendNotificationErrorApproveProjectInviteAsync("Ошибка",
                    "Ошибка при подтверждении приглашения в проект. Мы уже знаем о ней и разбираемся. " +
                    "А пока, попробуйте еще раз.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, token);
                
                throw ex;
            }

            await _projectNotificationsRepository.ApproveProjectInviteAsync(notificationId);

            await SendNotificationSuccessApproveProjectInviteAsync("Все хорошо", "Приглашение в проект успешно.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);
            
            var projectId = await _projectNotificationsRepository.GetProjectIdByNotificationIdAsync(notificationId);
            var projectName = await _projectRepository.GetProjectNameByProjectIdAsync(projectId);

            await AddNotificationApproveInviteProjectAsync(userId, projectId, projectName);
            
            await SendNotificationApproveInviteProjectAsync(notificationId, userId, projectId, projectName, account);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод реджектит приглашение в проект.
    /// </summary>
    /// <param name="notificationId">Id уведомления.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task RejectProjectInviteAsync(long notificationId, string account, string token)
    {
        try
        {
            if (notificationId <= 0)
            {
                var ex = new InvalidOperationException("Id уведомления был <= 0.");
                throw ex;
            }
            
            // Проверяем существование уведомления.
            var isExistsNotification = await _projectNotificationsRepository
                .CheckExistsNotificationByIdAsync(notificationId);
            
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            if (!isExistsNotification)
            {
                var ex = new InvalidOperationException(
                    $"Уведомления с NotificationId: {notificationId} не существует.");

                await SendNotificationErrorRejectProjectInviteAsync("Ошибка",
                    "Ошибка при отклонении приглашения в проект. Мы уже знаем о ней и разбираемся. " +
                    "А пока, попробуйте еще раз.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, token);
                
                throw ex;
            }

            await _projectNotificationsRepository.RejectProjectInviteAsync(notificationId);

            await SendNotificationSuccessRejectProjectInviteAsync("Все хорошо",
                "Отклонение приглашения в проект успешно.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);
            
            var projectId = await _projectNotificationsRepository.GetProjectIdByNotificationIdAsync(notificationId);
            var projectName = await _projectRepository.GetProjectNameByProjectIdAsync(projectId);

            await AddNotificationRejectInviteProjectAsync(projectId, userId, projectName);

            await SendNotificationRejectInviteProjectAsync(notificationId, userId, projectId, projectName, account);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод отправляет уведомление об ошибке при приглашении в проект по ссылке.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationErrorProjectInviteTeamByLinkAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationErrorProjectInviteTeamByLink",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление об ошибке при приглашении в проект по логину.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationErrorProjectInviteTeamByLoginAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationErrorProjectInviteTeamByLogin",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление об ошибке при приглашении в проект по почте.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationErrorProjectInviteTeamByEmailAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationErrorProjectInviteTeamByEmail",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление об ошибке при приглашении в проект по номеру телефону.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationErrorProjectInviteTeamByPhoneNumberAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationErrorProjectInviteTeamByPhoneNumber",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление предупреждения об при инвайте в проект, который находится на модерации.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationWarningProjectInviteTeamAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationWarningProjectInviteTeam",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление предупреждения о приглашенном пользователе в команде проекта.
    /// Повторно нельзя приглашать для избежания дублей.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationWarningUserAlreadyProjectInvitedTeamAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationWarningUserAlreadyProjectInvitedTeam",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление об успешном исключения пользователя из команды проекта.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationSuccessDeleteProjectTeamMemberAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationSuccessDeleteProjectTeamMember",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление об успехе при добавлении проекта в архив.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationSuccessAddProjectArchiveAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationSuccessAddProjectArchive",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление об ошибке при добавлении проекта в архив.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationErrorAddProjectArchiveAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationErrorAddProjectArchive",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление предупреждения о дубле при добавлении проекта в архив.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationWarningAddProjectArchiveAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationWarningAddProjectArchive",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление об ошибке при удалении проекта из архива.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationErrorDeleteProjectArchiveAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationErrorDeleteProjectArchive",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление об успехе при удалении проекта из архива.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationSuccessDeleteProjectArchiveAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationSuccessDeleteProjectArchive",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление предупреждения при удалении проекта из архива.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task SendNotificationWarningDeleteProjectArchiveAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationWarningDeleteProjectArchive",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод отправляет уведомление об ошибке при апруве приглашения в проект.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    private async Task SendNotificationErrorApproveProjectInviteAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationErrorApproveProjectInvite",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }
    
    /// <summary>
    /// Метод отправляет уведомление об успехе при приглашении в проект.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    private async Task SendNotificationSuccessApproveProjectInviteAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationSuccessApproveProjectInvite",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }
    
    /// <summary>
    /// Метод отправляет уведомление об ошибке при реджекте приглашения в проект.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    private async Task SendNotificationErrorRejectProjectInviteAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationErrorRejectProjectInvite",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }
    
    /// <summary>
    /// Метод отправляет уведомление об успехе при реджекте в проект.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="token">Токен пользователя.</param>
    private async Task SendNotificationSuccessRejectProjectInviteAsync(string title, string notifyText,
        string notificationLevel, string token)
    {
        var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

        await _hubContext.Clients
            .Client(connectionId)
            .SendAsync("SendNotificationSuccessRejectProjectInvite",
                new NotificationOutput
                {
                    Title = title,
                    Message = notifyText,
                    NotificationLevel = notificationLevel
                });
    }

    /// <summary>
    /// Метод отправляет уведомление в приложении о принятом приглашении в проект.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    private async Task AddNotificationApproveInviteProjectAsync(long userId, long projectId, string projectName)
    {
        var isProjectOwner = await _projectRepository.CheckProjectOwnerAsync(projectId, userId);
        
        await _projectNotificationsRepository.AddNotificationApproveInviteProjectAsync(projectId, null, userId,
            projectName, isProjectOwner);
    }

    /// <summary>
    /// Метод отправляет уведомление на почту о принятом приглашении в проект.
    /// </summary>
    /// <param name="notificationId">Id уведомление.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="account">Аккаунт.</param>
    private async Task SendNotificationApproveInviteProjectAsync(long notificationId, long userId, long projectId,
        string projectName, string account)
    {
        var vacancyId = await _projectNotificationsRepository.GetVacancyIdByNotificationIdAsync(notificationId);

        var vacancyName = string.Empty;
            
        if (vacancyId is not null)
        {
            vacancyName = await _vacancyRepository.GetVacancyNameByIdAsync((long)vacancyId);   
        }

        var user = await _userRepository.GetUserPhoneEmailByUserIdAsync(userId);
            
        var isEnabledEmailNotifications = await _globalConfigRepository
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.EmailNotifications.EMAIL_NOTIFICATIONS_DISABLE_MODE_ENABLED);
        var api = await _globalConfigRepository
            .GetValueByKeyAsync<string>(GlobalConfigKeys.EmailNotifications.API_MAIL_URL);
        
        await _mailingsService.SendNotificationApproveInviteProjectAsync(user.Email, projectId, projectName,
            vacancyName, account, isEnabledEmailNotifications, api);
    }

    /// <summary>
    /// Метод отправляет уведомление в приложении о отклонении приглашения в проект.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectName">Название проекта.</param>
    private async Task AddNotificationRejectInviteProjectAsync(long projectId, long userId, string projectName)
    {
        var isProjectOwner = await _projectRepository.CheckProjectOwnerAsync(projectId, userId);
        
        await _projectNotificationsRepository.AddNotificationRejectInviteProjectAsync(projectId, null, userId,
            projectName, isProjectOwner);
    }

    /// <summary>
    /// Метод отправляет уведомление на почту о отклонении приглашения в проект.
    /// </summary>
    /// <param name="notificationId">Id уведомление.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="account">Аккаунт.</param>
    private async Task SendNotificationRejectInviteProjectAsync(long notificationId, long userId, long projectId,
        string projectName, string account)
    {
        var vacancyId = await _projectNotificationsRepository.GetVacancyIdByNotificationIdAsync(notificationId);

        var vacancyName = string.Empty;
            
        if (vacancyId is not null)
        {
            vacancyName = await _vacancyRepository.GetVacancyNameByIdAsync((long)vacancyId);   
        }

        var user = await _userRepository.GetUserPhoneEmailByUserIdAsync(userId);
            
        var isEnabledEmailNotifications = await _globalConfigRepository
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.EmailNotifications.EMAIL_NOTIFICATIONS_DISABLE_MODE_ENABLED);
        var api = await _globalConfigRepository
            .GetValueByKeyAsync<string>(GlobalConfigKeys.EmailNotifications.API_MAIL_URL);
        
        await _mailingsService.SendNotificationRejectInviteProjectAsync(user.Email, projectId, projectName,
            vacancyName, account, isEnabledEmailNotifications, api);
    }

    #endregion
}