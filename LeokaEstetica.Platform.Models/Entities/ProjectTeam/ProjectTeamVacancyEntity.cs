namespace LeokaEstetica.Platform.Models.Entities.ProjectTeam;

/// <summary>
/// Классс сопоставляется с таблицей вакансий, которые включены в команду проекта Teams.ProjectsTeamsVacancies.
/// </summary>
public class ProjectTeamVacancyEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long TeamVacancyId { get; set; }

    /// <summary>
    /// Id вакансии.
    /// </summary>
    public long VacancyId { get; set; }

    /// <summary>
    /// Id команды.
    /// </summary>
    public long TeamId { get; set; }

    /// <summary>
    /// Признак активности вакансии.
    /// Например, если вакансия находится в архиве, то она не активна.
    /// </summary>
    public bool IsActive { get; set; }
}