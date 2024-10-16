using System.Data;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagmentHumanResources;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagementHumanResources;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagementHumanResources;
using LeokaEstetica.Platform.Models.Enums;
using Enum = LeokaEstetica.Platform.Models.Enums.Enum;

namespace LeokaEstetica.Platform.Database.Repositories.ProjectManagmentHumanResources;

/// <summary>
/// Класс реализует методы репозитория календарей.
/// </summary>
internal sealed class CalendarRepository : BaseRepository, ICalendarRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="connectionProvider">Провайдер БД.</param>
    public CalendarRepository(IConnectionProvider connectionProvider) : base(connectionProvider)
    {
    }

    #region Публичные методы

    /// <inheritdoc />
    public async Task<IEnumerable<CalendarOutput>?> GetCalendarEventsAsync(long userId)
     {
         using var connection = await ConnectionProvider.GetConnectionAsync();

         var parameters = new DynamicParameters();
         parameters.Add("@userId", userId);

         var query = "SELECT ec.event_id, " +
                     "ec.event_name, " +
                     "ec.event_description, " +
                     "ec.created_by, " +
                     "ec.created_at, " +
                     "ec.event_start_date, " +
                     "ec.event_end_date, " +
                     "ec.event_location " +
                     "FROM project_management_human_resources.calendar_events AS ec " +
                     "INNER JOIN project_management_human_resources.calendar_event_members AS em " +
                     "ON ec.event_id = em.event_id " +
                     "WHERE em.event_member_id = @userId";

         var result = await connection.QueryAsync<CalendarOutput>(query, parameters);

         return result;
     }

     /// <inheritdoc />
     public async Task<IEnumerable<EventMemberOutput>?> GetEventMembersAsync(IEnumerable<long> eventIds)
     {
         using var connection = await ConnectionProvider.GetConnectionAsync();

         var parameters = new DynamicParameters();
         parameters.Add("@eventIds", eventIds.AsList());

         var query = "SELECT id, " +
                     "event_member_id, " +
                     "event_id, " +
                     "member_status::project_management_human_resources.calendar_member_status_enum " +
                     "AS CalendarEventMemberStatus, " +
                     "joined " +
                     "FROM project_management_human_resources.calendar_event_members " +
                     "WHERE event_id = ANY (@eventIds)";
                     
         var result = await connection.QueryAsync<EventMemberOutput>(query, parameters);

         return result;
     }

     /// <inheritdoc />
     public async Task<IEnumerable<EventMemberRoleOutput>?> GetEventMemberRolesAsync(IEnumerable<long> eventIds,
         IEnumerable<long> eventMemberIds)
     {
         using var connection = await ConnectionProvider.GetConnectionAsync();

         var parameters = new DynamicParameters();
         parameters.Add("@eventIds", eventIds.AsList());
         parameters.Add("@eventMemberIds", eventMemberIds.AsList());
         
         var query = "SELECT role_id, " +
                     "event_id, " +
                     "role_name, " +
                     "role_sys_name, " +
                     "event_member_id " +
                     "FROM roles.calendar_event_role_members " +
                     "WHERE event_id = ANY (@eventIds) " +
                     "AND event_member_id = ANY (@eventMemberIds)";
                     
         var result = await connection.QueryAsync<EventMemberRoleOutput>(query, parameters);

         return result;
     }

     /// <inheritdoc />
     public async Task CreateCalendarEventAsync(CalendarInput calendarInput)
     {
         using var connection = await ConnectionProvider.GetConnectionAsync();
         using var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

         try
         {
             // Создаем событие.
             var eventId = await CreateEventAsync(calendarInput, connection);
             
             // Добавляем участников события.
             await CreateEventMembersAsync(eventId, calendarInput.EventMembers!,
                 System.Enum.Parse<CalendarEventMemberStatusEnum>(calendarInput.CalendarEventMemberStatus!),
                 connection);
             
             transaction.Commit();
         }
         
         catch
         {
             transaction.Rollback();
             throw;
         }
     }

     /// <inheritdoc />
     public async Task<CalendarOutput> GetEventDetailsAsync(long eventId)
     {
         using var connection = await ConnectionProvider.GetConnectionAsync();

         var parameters = new DynamicParameters();
         parameters.Add("@eventId", eventId);

         var eventQuery = "SELECT event_id, " +
                          "event_name, " +
                          "event_description, " +
                          "created_by, " +
                          "created_at, " +
                          "event_start_date, " +
                          "event_end_date, " +
                          "event_location " +
                          "FROM project_management_human_resources.calendar_events " +
                          "WHERE event_id = @eventId";

         var @event = await connection.QueryFirstOrDefaultAsync<CalendarOutput>(eventQuery, parameters);

         if (@event is null)
         {
             throw new InvalidOperationException("Ошибка при получении данных события календаря. " +
                                                 $"EventId: {eventId}.");
         }

         var eventMembersQuery = "SELECT em.id, " +
                                 "em.event_member_id, " +
                                 "em.event_id, " +
                                 "em.member_status::project_management_human_resources.calendar_member_status_enum " +
                                 "AS CalendarEventMemberStatus, " +
                                 "em.joined," +
                                 "u.\"Email\" " +
                                 "FROM project_management_human_resources.calendar_event_members AS em " +
                                 "INNER JOIN dbo.\"Users\" AS u " +
                                 "ON em.event_member_id = u.\"UserId\" " +
                                 "WHERE em.event_id = @eventId";

         var eventMembers = (await connection.QueryAsync<EventMemberOutput>(eventMembersQuery, parameters))
             ?.AsList();

         if (eventMembers is not null && eventMembers.Count > 0)
         {
             @event.EventMembers ??= new List<EventMemberOutput>();
             @event.EventMembers = eventMembers;
         }

         return @event;
     }

     /// <inheritdoc />
     public async Task UpdateEventAsync(CalendarInput calendarInput)
     {
         using var connection = await ConnectionProvider.GetConnectionAsync();
         using var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

         try
         {
             // Обновляем событие.
             await UpdateEventAsync(calendarInput, connection);
             
             // Обновляем участников события.
             await ActualizeEventMembersAsync(calendarInput.EventId!.Value, calendarInput.EventMembers!,
                 System.Enum.Parse<CalendarEventMemberStatusEnum>(calendarInput.CalendarEventMemberStatus!),
                 connection);
             
             transaction.Commit();
         }
         
         catch
         {
             transaction.Rollback();
             throw;
         }
     }

     /// <inheritdoc />
     public async Task RemoveEventAsync(long eventId)
     {
         using var connection = await ConnectionProvider.GetConnectionAsync();
         using var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

         try
         {
             var parameters = new DynamicParameters();
             parameters.Add("@eventId", eventId);

             var removeMemberRolesQuery = "DELETE FROM roles.calendar_event_role_members " +
                                          "WHERE event_id = @eventId";
             
             await connection.ExecuteAsync(removeMemberRolesQuery, parameters);

             var removeMembersQuery = "DELETE FROM project_management_human_resources.calendar_event_members " +
                                      "WHERE event_id = @eventId";

             await connection.ExecuteAsync(removeMembersQuery, parameters);

             var removeEventQuery = "DELETE FROM project_management_human_resources.calendar_events " +
                                    "WHERE event_id = @eventId";
                                    
             await connection.ExecuteAsync(removeEventQuery, parameters);
             
             transaction.Commit();
         }
         
         catch
         {
             transaction.Rollback();
             throw;
         }
     }

     #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод создает событие.
    /// </summary>
    /// <param name="calendarInput">Входная модель.</param>
    /// <param name="connection">Подключение к БД.</param>
    /// <returns>Id события.</returns>
    private async Task<long> CreateEventAsync(CalendarInput calendarInput, IDbConnection connection)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@eventName", calendarInput.EventName);

        var query = "INSERT INTO project_management_human_resources.calendar_events (event_name";

        if (!string.IsNullOrWhiteSpace(calendarInput.EventDescription))
        {
            query += " ,event_description";
            parameters.Add("@eventDescription", calendarInput.EventDescription);
        }

        query += ", created_by, event_start_date, event_end_date";
        parameters.Add("@createdBy", calendarInput.CreatedBy);
        parameters.Add("@eventStartDate", calendarInput.EventStartDate);
        parameters.Add("@eventEndDate", calendarInput.EventEndDate);
         
        if (!string.IsNullOrWhiteSpace(calendarInput.EventLocation))
        {
            query += " ,event_location";
            parameters.Add("@eventLocation", calendarInput.EventLocation);
        }

        query += ") VALUES (@eventName, ";
         
        if (!string.IsNullOrWhiteSpace(calendarInput.EventDescription))
        {
            query += " @eventDescription";
        }

        query += ", @createdBy, @eventStartDate, @eventEndDate";
         
        if (!string.IsNullOrWhiteSpace(calendarInput.EventLocation))
        {
            query += ", @eventLocation";
        }

        query += ") RETURNING event_id";

        var eventId = await connection.ExecuteScalarAsync<long>(query, parameters);

        return eventId;
    }

    /// <summary>
    /// Метод добавляет участников события.
    /// </summary>
    /// <param name="eventId">Id события.</param>
    /// <param name="eventMembers">Список участников события.</param>
    /// <param name="calendarEventMemberStatus">Статус.</param>
    /// <param name="connection">Подключение к БД.</param>
    private async Task CreateEventMembersAsync(long eventId, List<EventMemberInput> eventMembers,
        CalendarEventMemberStatusEnum calendarEventMemberStatus, IDbConnection connection)
    {
        var parameters = new List<DynamicParameters>();

        foreach (var p in eventMembers)
        {
            var tempParameters = new DynamicParameters();
            tempParameters.Add("@eventId", eventId);
            tempParameters.Add("@eventMemberId", p.EventMemberId);
            tempParameters.Add("@memberStatus", new Enum(calendarEventMemberStatus));
            tempParameters.Add("@joined", DateTime.UtcNow);

            parameters.Add(tempParameters);
        }

        var query = "INSERT INTO project_management_human_resources.calendar_event_members " +
                    "(event_member_id, event_id, member_status, joined) " +
                    "VALUES (@eventMemberId, @eventId, @memberStatus, @joined)";

        await connection.ExecuteAsync(query, parameters);
    }
    
    /// <summary>
    /// Метод обновляет событие.
    /// </summary>
    /// <param name="calendarInput">Входная модель.</param>
    /// <param name="connection">Подключение к БД.</param>
    /// <returns>Id события.</returns>
    private async Task UpdateEventAsync(CalendarInput calendarInput, IDbConnection connection)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@eventName", calendarInput.EventName);

        var query = "UPDATE project_management_human_resources.calendar_events " +
                    "SET event_name = @eventName";

        if (!string.IsNullOrWhiteSpace(calendarInput.EventDescription))
        {
            query += ", event_description = @eventDescription";
            parameters.Add("@eventDescription", calendarInput.EventDescription);
        }

        query += ", created_by = @createdBy, " +
                 "event_start_date = @eventStartDate, " +
                 "event_end_date = @eventEndDate";
        
        parameters.Add("@createdBy", calendarInput.CreatedBy);
        parameters.Add("@eventStartDate", calendarInput.EventStartDate);
        parameters.Add("@eventEndDate", calendarInput.EventEndDate);
         
        if (!string.IsNullOrWhiteSpace(calendarInput.EventLocation))
        {
            query += " ,event_location = @eventLocation";
            parameters.Add("@eventLocation", calendarInput.EventLocation);
        }

        parameters.Add("@eventId", calendarInput.EventId!.Value);
        query += " WHERE event_id = @eventId";

        await connection.ExecuteAsync(query, parameters);
    }
    
    /// <summary>
    /// Метод актуализирует участников события.
    /// </summary>
    /// <param name="eventId">Id события.</param>
    /// <param name="eventMembers">Список участников события.</param>
    /// <param name="calendarEventMemberStatus">Статус.</param>
    /// <param name="connection">Подключение к БД.</param>
    private async Task ActualizeEventMembersAsync(long eventId, List<EventMemberInput> eventMembers,
        CalendarEventMemberStatusEnum CalendarEventMemberStatus, IDbConnection connection)
    {
        // Удаляем старых участников события.
        var removeParameters = new DynamicParameters();
        removeParameters.Add("@eventId", eventId);

        var removeQuery = "DELETE FROM project_management_human_resources.calendar_event_members " +
                          "WHERE event_id = @eventId";
        
        await connection.ExecuteAsync(removeQuery, removeParameters);
        
        var addParameters = new List<DynamicParameters>();

        // Добавляем новых участников события.
        foreach (var p in eventMembers)
        {
            var tempParameters = new DynamicParameters();
            tempParameters.Add("@eventId", eventId);
            tempParameters.Add("@eventMemberId", p.EventMemberId);
            tempParameters.Add("@memberStatus", new Enum(CalendarEventMemberStatus));
            tempParameters.Add("@joined", DateTime.UtcNow);

            addParameters.Add(tempParameters);
        }

        var addQuery = "INSERT INTO project_management_human_resources.calendar_event_members " +
                       "(event_member_id, event_id, member_status, joined) " +
                       "VALUES (@eventMemberId, @eventId, @memberStatus, @joined)";

        await connection.ExecuteAsync(addQuery, addParameters);
    }

    #endregion
}