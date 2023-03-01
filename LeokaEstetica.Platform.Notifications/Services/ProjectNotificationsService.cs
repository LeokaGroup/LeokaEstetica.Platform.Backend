using AutoMapper;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Notification;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Notification;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Notifications.Data;
using Microsoft.AspNetCore.SignalR;
using NotificationOutput = LeokaEstetica.Platform.Notifications.Models.Output.NotificationOutput;
using NotificationProjectOutput = LeokaEstetica.Platform.Models.Dto.Output.Notification.NotificationOutput;

namespace LeokaEstetica.Platform.Notifications.Services;

/// <summary>
/// Класс реализует методы уведомлений проектов.
/// </summary>
public class ProjectNotificationsService : IProjectNotificationsService
{
    private readonly IHubContext<NotifyHub> _hubContext;
    private readonly ILogService _logService;
    private readonly IUserRepository _userRepository;
    private readonly IProjectNotificationsRepository _projectNotificationsRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="hubContext">Контекст хаба.</param>
    /// <param name="logService">Сервис логера.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="_projectNotificationsRepository">Репозиторий уведомлений проектов.</param>
    /// <param name="mapper">Автомаппер.</param>
    public ProjectNotificationsService(IHubContext<NotifyHub> hubContext, 
        ILogService logService, 
        IUserRepository userRepository,
        IMapper mapper, 
        IProjectNotificationsRepository projectNotificationsRepository)
    {
        _hubContext = hubContext;
        _logService = logService;
        _userRepository = userRepository;
        _mapper = mapper;
        _projectNotificationsRepository = projectNotificationsRepository;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод отправляет уведомление об успешном создании проекта пользователя.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    public async Task SendNotificationSuccessCreatedUserProjectAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationSuccessCreatedUserProject", new NotificationOutput
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
    public async Task SendNotificationErrorCreatedUserProjectAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationErrorCreatedUserProject", new NotificationOutput
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
    public async Task SendNotificationWarningDublicateUserProjectAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationWarningDublicateUserProject", new NotificationOutput
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
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotificationSuccessUpdatedUserProjectAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationSuccessUpdatedUserProject", new NotificationOutput
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
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotificationErrorUpdatedUserProjectAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationErrorUpdatedUserProject", new NotificationOutput
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
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotificationSuccessAttachProjectVacancyAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationSuccessAttachProjectVacancy", new NotificationOutput
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
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotificationErrorDublicateAttachProjectVacancyAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationErrorDublicateAttachProjectVacancy",
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
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotificationSuccessProjectResponseAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationSuccessProjectResponse",
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
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotificationWarningProjectResponseAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationWarningProjectResponse",
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
    /// <param name="userCode">Код пользователя.</param>
    /// <param name="searchText">Поисковый запрос.</param>
    public async Task SendNotificationWarningSearchProjectTeamMemberAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationWarningSearchProjectTeamMember",
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
    /// <param name="userCode">Код пользователя.</param>
    /// <param name="searchText">Поисковый запрос.</param>
    public async Task SendNotificationWarningInviteProjectTeamMembersAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationWarningInviteProjectTeamMembers",
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
    /// <param name="userCode">Код пользователя.</param>
    /// <param name="searchText">Поисковый запрос.</param>
    public async Task SendNotificationErrorInviteProjectTeamMembersAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationErrorInviteProjectTeamMembers",
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
    public async Task SendNotificationWarningLimitFareRuleProjectsAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationWarningLimitFareRuleProjects",
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
    public async Task SendNotificationErrorDeleteProjectVacancyAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationErrorDeleteProjectVacancy",
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
    public async Task SendNotificationSuccessDeleteProjectVacancyAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationSuccessDeleteProjectVacancy",
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
    public async Task SendNotificationErrorDeleteProjectAsync(string title, string notifyText, 
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationErrorDeleteProject",
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
    public async Task SendNotificationSuccessDeleteProjectAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationSuccessDeleteProject",
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
    /// <param name="userCode">Код пользователя.</param>
    public async Task SendNotificationErrorProjectResponseAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationErrorProjectResponse",
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

            var result = new NotificationResultOutput
            {
                Notifications =
                    new List<NotificationProjectOutput>(items.UserNotifications.Count + items.OwnerNotifications.Count)
            };

            if (items.Item1.Any())
            {
                result.Notifications = _mapper.Map<List<NotificationProjectOutput>>(items.UserNotifications);
            }
            
            // Работаем с уведомлениями владельцев.
            if (items.Item2.Any())
            {
                var newOwnerNotifications = _mapper.Map<List<NotificationProjectOutput>>(items.OwnerNotifications);
                var ownerNotifications = FillOwnerNotificationsFlags(newOwnerNotifications);
                result.Notifications.AddRange(ownerNotifications);
            }

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод апрувит приглашение в проект.
    /// </summary>
    /// <param name="notificationId">Id уведомления.</param>
    public async Task ApproveProjectInviteAsync(long notificationId)
    {
        try
        {
            // Проверяем существование уведомления.
            var isExistsNotification =
                await _projectNotificationsRepository.CheckExistsNotificationByIdAsync(notificationId);

            if (!isExistsNotification)
            {
                var ex = new InvalidOperationException(
                    $"Уведомления с NotificationId: {notificationId} не существует.");

                await SendNotificationErrorApproveProjectInviteAsync("Ошибка",
                    "Ошибка при подтверждении приглашения в проект. Мы уже знаем о ней и разбираемся. " +
                    "А пока, попробуйте еще раз.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR);
                
                throw ex;
            }

            await _projectNotificationsRepository.ApproveProjectInviteAsync(notificationId);
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    #endregion

    #region Приватные методы.
    
    /// <summary>
    /// Метод проставляет признак отображения кнопок уведомлений для владельцев.
    /// </summary>
    /// <param name="ownerNotifications">Уведомления владельцев.</param>
    /// <returns>Список уведомлений.</returns>
    private List<NotificationProjectOutput> FillOwnerNotificationsFlags(
        List<NotificationProjectOutput> ownerNotifications)
    {
        // Проставляем признак отображения кнопок для владельцев.
        foreach (var notification in ownerNotifications)
        {
            notification.IsVisibleNotificationsButtons = true;
        }

        return ownerNotifications;
    }
    
    /// <summary>
    /// Метод отправляет уведомление об ошибке при апруве приглашения в проект.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="notifyText">Текст уведомления.</param>
    /// <param name="notificationLevel">Уровень уведомления.</param>
    /// <param name="userCode">Код пользователя.</param>
    private async Task SendNotificationErrorApproveProjectInviteAsync(string title, string notifyText,
        string notificationLevel)
    {
        await _hubContext.Clients.All.SendAsync("SendNotificationErrorApproveProjectInvite",
            new NotificationOutput
            {
                Title = title,
                Message = notifyText,
                NotificationLevel = notificationLevel
            });
    }

    #endregion
}