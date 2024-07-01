namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectTeam;

/// <summary>
/// Класс входной модели приглашения в команду проекта.
/// </summary>
public class InviteProjectMemberInput
{
    /// <summary>
    /// Текст, который будет использоваться для поиска пользователя для приглашения.
    /// </summary>
    public string? InviteText { get; set; }

    /// <summary>
    /// Способ приглашения.
    /// </summary>
    public string? InviteType { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Id вакансии.
    /// Если вакансия не передана, значит идет инвайт без указания вакансии.
    /// </summary>
    public long? VacancyId { get; set; }
}