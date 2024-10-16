using System.Runtime.CompilerServices;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagmentHumanResources;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagementHumanResources;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagementHumanResources;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.ProjectManagement.HumanResources.Abstractions;
using Enum = System.Enum;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.ProjectManagement.HumanResources.Services;

/// <summary>
/// Класс реализует методы сервиса календарей.
/// </summary>
internal sealed class CalendarService : ICalendarService
{
    private readonly ILogger<CalendarService>? _logger;
    private readonly IUserRepository _userRepository;
    private readonly ICalendarRepository _calendarRepository;
    private readonly Lazy<IHubNotificationService> _hubNotificationService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="calendarRepository">Репозиторий календаря.</param>
    /// <param name="hubNotificationService">Сервис уведомлений хабов.</param>
    public CalendarService(ILogger<CalendarService>? logger,
        IUserRepository userRepository,
        ICalendarRepository calendarRepository,
        Lazy<IHubNotificationService> hubNotificationService)
    {
        _logger = logger;
        _userRepository = userRepository;
        _calendarRepository = calendarRepository;
        _hubNotificationService = hubNotificationService;
    }

    #region Публичные методы.

    /// <inheritdoc />
    public async Task<IEnumerable<CalendarOutput>> GetCalendarEventsAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }

            // Получаем список событий, где в участниках есть текущий пользователь.
            var events = (await _calendarRepository.GetCalendarEventsAsync(userId))?.AsList();

            if (events is null || events.Count == 0)
            {
                return Enumerable.Empty<CalendarOutput>();
            }

            // Получаем участников событий.
            var eventMembers = (await _calendarRepository.GetEventMembersAsync(
                    events.Select(x => x.EventId)))
                ?.AsList();

            var isAnyEventMembers = eventMembers is not null && eventMembers.Count > 0;

            // Получаем роли участников событий.
            var eventMemberRoles = (await _calendarRepository.GetEventMemberRolesAsync(
                    events.Select(x => x.EventId), eventMembers?.Select(x => x.EventMemberId) ?? new List<long>()))
                ?.AsList();

            var isAnyeventMemberRoles = eventMembers is not null && eventMembers.Count > 0;

            // Заполняем данные каждого события.
            foreach (var e in events)
            {
                if (isAnyEventMembers)
                {
                    e.EventMembers ??= new List<EventMemberOutput>();
                    e.EventMembers.AddRange(eventMembers?.Where(x => x.EventId == e.EventId) ??
                                            new List<EventMemberOutput>());
                }

                if (isAnyeventMemberRoles)
                {
                    e.EventMemberRoles ??= new List<EventMemberRoleOutput>();
                    e.EventMemberRoles.AddRange(eventMemberRoles?.Where(x => x.EventId == e.EventId) ??
                                                new List<EventMemberRoleOutput>());
                }
            }

            return events;
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task CreateCalendarEventAsync(CalendarInput calendarInput, string account)
    {  
        var userId = await _userRepository.GetUserByEmailAsync(account);
        var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);
        try
        {
            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }

            calendarInput.EventMembers ??= new List<EventMemberInput>();

            var eventMemberMails = calendarInput.EventMembers.Select(x => x.EventMemberMail)?.AsList();

            eventMemberMails ??= new List<string?>();
            
            var eventMembersDict = await _userRepository.GetUserByEmailsAsync(eventMemberMails);
            
            // Добавляем в участники события текущего пользователя, который создает событие.
            eventMembersDict.TryAdd(account, userId);

            foreach (var em in calendarInput.EventMembers)
            {
                em.EventMemberId = eventMembersDict!.TryGet(em.EventMemberMail);
            }

            calendarInput.CreatedBy = userId;
            
            await _calendarRepository.CreateCalendarEventAsync(calendarInput);
            
            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Событие календаря успешно создано.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotifySuccessCreateCalendarEvent",
                userCode, UserConnectionModuleEnum.Main);
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
        
            await _hubNotificationService.Value.SendNotificationAsync("Что то пошло не так",
                "Ошибка при сохранении данных. Мы уже знаем о проблеме и разбираемся с ней.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotifyErrorCreateCalendarEvent",
                userCode, UserConnectionModuleEnum.Main);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<CalendarOutput> GetEventDetailsAsync(long eventId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }
            
            var result = await _calendarRepository.GetEventDetailsAsync(eventId);

            result.EventMembers ??= new List<EventMemberOutput>();

            foreach (var em in result.EventMembers)
            {
                em.DisplayEventMemberStatus = Enum.Parse<CalendarEventMemberStatusEnum>(
                    em.CalendarEventMemberStatusValue.ToString()).GetEnumDescription();

                if (em.EventMemberId == userId)
                {
                    result.DisplayEventMemberStatus = Enum.Parse<CalendarEventMemberStatusEnum>(
                        em.CalendarEventMemberStatusValue.ToString()).GetEnumDescription();
                    result.CalendarEventMemberStatusValue = em.CalendarEventMemberStatusValue;
                }
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateEventAsync(CalendarInput calendarInput, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }
            
            calendarInput.EventMembers ??= new List<EventMemberInput>();
            
            var eventMemberMails = calendarInput.EventMembers.Select(x => x.EventMemberMail);
            var eventMembersDict = await _userRepository.GetUserByEmailsAsync(eventMemberMails);
            
            // Добавляем в участники события текущего пользователя, который создает событие.
            eventMembersDict.TryAdd(account, userId);

            foreach (var em in calendarInput.EventMembers)
            {
                em.EventMemberId = eventMembersDict!.TryGet(em.EventMemberMail);
            }

            calendarInput.CreatedBy = userId;
            
            await _calendarRepository.UpdateEventAsync(calendarInput);
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task RemoveEventAsync(long eventId, string account)
    {
        var userId = await _userRepository.GetUserByEmailAsync(account);
        var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);
        try
        {
            await _calendarRepository.RemoveEventAsync(eventId);
            
            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Событие календаря успешно удалено.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotifySuccessRemoveCalendarEvent",
                userCode, UserConnectionModuleEnum.Main);
        }
        
        catch (Exception ex)
        {
             _logger?.LogError(ex, ex.Message);
             await _hubNotificationService.Value.SendNotificationAsync("Что то пошло не так",
                 "Ошибка при удалении события календаря.",
                 NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotifyErrorRemoveCalendarEvent",
                 userCode, UserConnectionModuleEnum.Main);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}