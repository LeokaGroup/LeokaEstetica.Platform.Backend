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
                     "member_status::project_management_human_resources.CALENDAR_MEMBER_STATUS_ENUM " +
                     "AS CalendarEventMemberStatusValue, " +
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
                 calendarInput.CalendarEventMemberStatus, connection);
             
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
    /// <param name="connection">Транзакция.</param>
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
            query += ", @eventDescription";
        }

        query += ", @createdBy, @eventStartDate, @eventEndDate";
         
        if (!string.IsNullOrWhiteSpace(calendarInput.EventLocation))
        {
            query += ", @eventLocation)";
        }

        query += " RETURNING event_id";

        var eventId = await connection.ExecuteScalarAsync<long>(query, parameters);

        return eventId;
    }

    /// <summary>
    /// Метод добавляет участников события.
    /// </summary>
    /// <param name="eventId">Id события.</param>
    /// <param name="eventMembers"></param>
    /// <param name="CalendarEventMemberStatus"></param>
    /// <param name="connection"></param>
    private async Task CreateEventMembersAsync(long eventId, List<EventMemberInput> eventMembers,
        CalendarEventMemberStatusEnum CalendarEventMemberStatus, IDbConnection connection)
    {
        var parameters = new List<DynamicParameters>();

        foreach (var p in eventMembers)
        {
            var tempParameters = new DynamicParameters();
            tempParameters.Add("@eventId", eventId);
            tempParameters.Add("@eventMemberId", p.EventMemberId);
            tempParameters.Add("@memberStatus", new Enum(CalendarEventMemberStatus));

            parameters.Add(tempParameters);
        }

        var query = "INSERT INTO project_management_human_resources.calendar_event_members " +
                    "(event_member_id, event_id, member_status) " +
                    "VALUES (@eventMemberId, @eventId, @memberStatus)";

        await connection.ExecuteAsync(query, parameters);
    }

    #endregion
}