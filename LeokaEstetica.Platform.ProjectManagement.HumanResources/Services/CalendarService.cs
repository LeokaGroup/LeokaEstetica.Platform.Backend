using System.Runtime.CompilerServices;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagmentHumanResources;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagementHumanResources;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagementHumanResources;
using LeokaEstetica.Platform.Models.Enums;
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

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="calendarRepository">Репозиторий календаря.</param>
    public CalendarService(ILogger<CalendarService>? logger,
        IUserRepository userRepository,
        ICalendarRepository calendarRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _calendarRepository = calendarRepository;
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
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }

            calendarInput.EventMembers ??= new List<EventMemberInput>();

            //Заполняем UserId
            var eventMemberIdList = (await _userRepository.GetUserByEmailsAsync(
            calendarInput.EventMembers.Select(x => x.EventMemberMail).ToList())).ToList();

            if (eventMemberIdList.Count != calendarInput.EventMembers.Count) 
                throw new ArgumentException();
            
            for (var i = 0; i < calendarInput.EventMembers.Count; i++)
            { 
                calendarInput.EventMembers[i].EventMemberId = eventMemberIdList[i];
            }
            
            // Добавляем в участники события текущего пользователя, который создает событие.
            if (calendarInput.EventMembers.Count == 0
                || !calendarInput.EventMembers.Select(x => x.EventMemberMail).Contains(account))
            {
                calendarInput.EventMembers.Add(new EventMemberInput
                {
                    EventMemberId = userId
                });
            }

            calendarInput.CreatedBy = userId;
            
            await _calendarRepository.CreateCalendarEventAsync(calendarInput);
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
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
            
            //Заполняем UserId
            var eventMemberIdList = (await _userRepository.GetUserByEmailsAsync(
                calendarInput.EventMembers.Select(x => x.EventMemberMail).ToList())).ToList();

            if (eventMemberIdList.Count != calendarInput.EventMembers.Count) 
                throw new ArgumentException();
            
            for (var i = 0; i < calendarInput.EventMembers.Count; i++)
            { 
                calendarInput.EventMembers[i].EventMemberId = eventMemberIdList[i];
            }
            
            // Добавляем в участники события текущего пользователя, который создает событие.
            if (calendarInput.EventMembers.Count == 0
                || !calendarInput.EventMembers.Select(x => x.EventMemberMail).Contains(account))
            {
                calendarInput.EventMembers.Add(new EventMemberInput
                {
                    EventMemberId = userId
                });
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
    public async Task RemoveEventAsync(long eventId)
    {
        try
        {
            await _calendarRepository.RemoveEventAsync(eventId);
        }
        
        catch (Exception ex)
        {
             _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}