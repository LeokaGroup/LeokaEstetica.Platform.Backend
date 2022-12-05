namespace LeokaEstetica.Platform.Models.Entities.Project;

/// <summary>
/// Класс сопоставляется с таблицей откликов на проекты Projects.ProjectResponses.
/// </summary>
public class ProjectResponseEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long ResponseId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Дата отклика на проект.
    /// </summary>
    public DateTime DateResponse { get; set; }

    /// <summary>
    /// Id статуса отклика.
    /// </summary>
    public int ProjectResponseStatuseId { get; set; }

    /// <summary>
    /// Id вакансии. Если был отклик с указанием вакансии проекта.
    /// </summary>
    public long? VacancyId { get; set; }
}