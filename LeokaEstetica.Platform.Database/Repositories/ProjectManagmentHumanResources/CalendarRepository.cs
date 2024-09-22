using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagmentHumanResources;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagementHumanResources;

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
}