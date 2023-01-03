namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectTeam;

/// <summary>
/// Класс входной модели приглашения в команду проекта.
/// </summary>
public class InviteProjectMemberInput
{
    /// <summary>
    /// Пользователь, который будет добавлен в команду проекта.
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Id вакансии.
    /// </summary>
    public long VacancyId { get; set; }
}