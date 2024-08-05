using System.Runtime.CompilerServices;
using AutoMapper;
using LeokaEstetica.Platform.Base.Abstractions.Messaging.Mail;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.Notification;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Notification;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using Microsoft.Extensions.Logging;
using NotificationProjectOutput = LeokaEstetica.Platform.Models.Dto.Output.Notification.NotificationOutput;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Notifications.Services;

/// <summary>
/// Класс реализует методы уведомлений проектов.
/// </summary>
internal sealed class ProjectNotificationsService : IProjectNotificationsService
{
    private readonly ILogger<ProjectNotificationsService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IProjectNotificationsRepository _projectNotificationsRepository;
    private readonly IMapper _mapper;
    private readonly IProjectRepository _projectRepository;
    private readonly IMailingsService _mailingsService;
    private readonly IGlobalConfigRepository _globalConfigRepository;
    private readonly IVacancyRepository _vacancyRepository;
    private readonly IProjectManagmentRepository _projectManagmentRepository;
    private readonly Lazy<IProjectManagementSettingsRepository> _projectManagementSettingsRepository;
    private readonly Lazy<IHubNotificationService> _hubNotificationService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logService">Сервис логера.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="projectNotificationsRepository">Репозиторий уведомлений проектов.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="projectManagmentRepository">Репозиторий модуля УП.</param>
    /// <param name="projectManagementSettingsRepository">Репозиторий настроек модуля УП.</param>
    /// <param name="hubNotificationService">Автомаппер.</param>
    public ProjectNotificationsService(ILogger<ProjectNotificationsService> logger, 
        IUserRepository userRepository,
        IMapper mapper, 
        IProjectNotificationsRepository projectNotificationsRepository, 
        IProjectRepository projectRepository, 
        IMailingsService mailingsService, 
        IGlobalConfigRepository globalConfigRepository, 
        IVacancyRepository vacancyRepository,
        IProjectManagmentRepository projectManagmentRepository,
        Lazy<IProjectManagementSettingsRepository> projectManagementSettingsRepository,
        Lazy<IHubNotificationService> hubNotificationService)
    {
        _logger = logger;
        _userRepository = userRepository;
        _mapper = mapper;
        _projectNotificationsRepository = projectNotificationsRepository;
        _projectRepository = projectRepository;
        _mailingsService = mailingsService;
        _globalConfigRepository = globalConfigRepository;
        _vacancyRepository = vacancyRepository;
        _projectManagmentRepository = projectManagmentRepository;
        _projectManagementSettingsRepository = projectManagementSettingsRepository;
        _hubNotificationService = hubNotificationService;
    }

    #region Публичные методы.

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
    public async Task ApproveProjectInviteAsync(long notificationId, string account)
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

            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

            if (!isExistsNotification)
            {
                var ex = new InvalidOperationException(
                    $"Уведомления с NotificationId: {notificationId} не существует.");

                await _hubNotificationService.Value.SendNotificationAsync("Ошибка",
                    "Ошибка при подтверждении приглашения в проект. Мы уже знаем о ней и разбираемся. " +
                    "А пока, попробуйте еще раз.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorApproveProjectInvite",
                    userCode, UserConnectionModuleEnum.ProjectManagement);

                throw ex;
            }

            // Принимаем приглашение в проект.
            await _projectNotificationsRepository.ApproveProjectInviteAsync(notificationId);

            var projectId = await _projectNotificationsRepository.GetProjectIdByNotificationIdAsync(notificationId);
            var projectName = await _projectRepository.GetProjectNameByProjectIdAsync(projectId);
            
            // Получаем Id команды проекта.
            var teamId = await _projectRepository.GetProjectTeamIdAsync(projectId);

            // Получаем данные уведомления.
            var notification = await _projectNotificationsRepository.GetProjectInviteNotificationAsync(notificationId);

            if (notification is null)
            {
                throw new InvalidOperationException("Не удалось получить данные уведомления. " +
                                                    $"NotificationId: {notificationId}.");
            }
            
            // TODO: Когда будет сделан выбор роли при приглашении, то тут конкретная роль будет передаваться.
            // Добавляем пользователя в команду проекта.
            var result = await _projectRepository.AddProjectTeamMemberAsync(notification.UserId,
                notification.VacancyId, teamId, "Участник");
            
            // Добавляем участника в раб.пространство проекта.
            await _projectManagmentRepository.AddProjectWorkSpaceMemberAsync(projectId, result.MemberId);

            var organizationId = await _projectManagmentRepository.GetCompanyIdByProjectIdAsync(projectId);
            
            // Добавляем участнику проекта роли.
            await _projectManagementSettingsRepository.Value.AddCompanyMemberRolesAsync(organizationId,
                result.MemberId);

            // Отправляем уведомление в приложении о принятом приглашении в проект.
            await AddNotificationApproveInviteProjectAsync(userId, projectId, projectName);
            
            // Отправляем уведомление на почту о принятом приглашении в проект.
            await SendNotificationApproveInviteProjectAsync(notificationId, userId, projectId, projectName, account);

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо", "Приглашение в проект успешно.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotificationSuccessApproveProjectInvite",
                userCode, UserConnectionModuleEnum.Main);
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод реджектит приглашение в проект.
    /// </summary>
    /// <param name="notificationId">Id уведомления.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task RejectProjectInviteAsync(long notificationId, string account)
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
            
            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

            if (!isExistsNotification)
            {
                var ex = new InvalidOperationException(
                    $"Уведомления с NotificationId: {notificationId} не существует.");

                await _hubNotificationService.Value.SendNotificationAsync("Ошибка",
                    "Ошибка при отклонении приглашения в проект. Мы уже знаем о ней и разбираемся. " +
                    "А пока, попробуйте еще раз.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorRejectProjectInvite",
                    userCode, UserConnectionModuleEnum.Main);

                throw ex;
            }

            await _projectNotificationsRepository.RejectProjectInviteAsync(notificationId);

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Отклонение приглашения в проект успешно.", NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS,
                "SendNotificationSuccessRejectProjectInvite", userCode, UserConnectionModuleEnum.Main);

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

    #endregion

    #region Приватные методы.

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