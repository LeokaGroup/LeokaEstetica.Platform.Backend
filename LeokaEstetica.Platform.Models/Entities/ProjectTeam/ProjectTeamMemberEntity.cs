using LeokaEstetica.Platform.Models.Entities.User;
using LeokaEstetica.Platform.Models.Entities.Vacancy;

namespace LeokaEstetica.Platform.Models.Entities.ProjectTeam;

/// <summary>
/// Класс сопоставляется с таблицей участников проекта Teams.ProjectsTeamsMembers.
/// </summary>
public class ProjectTeamMemberEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long MemberId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long TeamId { get; set; }

    /// <summary>
    /// FK.
    /// </summary>
    public ProjectTeamEntity ProjectTeam { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// FK.
    /// </summary>
    public UserEntity User { get; set; }

    /// <summary>
    /// Дата присоединения участника к команде.
    /// </summary>
    public DateTime Joined { get; set; }

    /// <summary>
    /// Id вакансии.
    /// </summary>
    public long VacancyId { get; set; }

    /// <summary>
    /// FK.
    /// </summary>
    public UserVacancyEntity UserVacancy { get; set; }
}