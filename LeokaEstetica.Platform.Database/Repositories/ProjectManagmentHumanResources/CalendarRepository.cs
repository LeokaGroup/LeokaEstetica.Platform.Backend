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
    public CalendarRepository(IConnectionProvider connectionProvider) : base(connectionProvider)
    {
    }
    
     /// <inheritdoc />
    public async Task<IEnumerable<CalendarOutput>> GetCalendarEventsAsync(long userId)
     {
         using var connection = await ConnectionProvider.GetConnectionAsync();

         var parameters = new DynamicParameters();
         parameters.Add("@userId", userId);

         var query = "SELECT ce.event_id AS EventId, " +
                     "ce.event_name, " +
                     "ce.event_description, " +
                     "ce.created_by, " +
                     "ce.created_at, " +
                     "ce.event_start_date, " +
                     "ce.event_end_date, " +
                     "ce.event_location, " +
                     "em.id AS member_id, " +
                     "em.event_member_id, " +
                     "em.member_status " +
                     "FROM project_management_human_resources.calendar_events AS ce " +
                     "INNER JOIN project_management_human_resources.calendar_event_members AS em " +
                     "ON ce.event_id = em.event_id " +
                     "WHERE em.event_member_id = @userId";
         
         var result = await connection.QueryAsync<CalendarOutput, EventMemberOutput, EventMemberRoleOutput, CalendarOutput>(query,
             (calendar, eventMember, eventMemberRole) =>
             {
                 calendar.EventMembers ??= new List<EventMemberOutput>();
                 calendar.EventMembers.Add(eventMember);

                 calendar.EventMemberRoles ??= new List<EventMemberRoleOutput>();
                 calendar.EventMemberRoles.Add(eventMemberRole);
                 
                 return calendar;
             }, parameters, splitOn: "EventId");

         return result;
     }
}