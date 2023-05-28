namespace LeokaEstetica.Platform.Models.Dto.Input.Project;

/// <summary>
/// Класс входной модели отклика на проект.
/// </summary>
public class ProjectResponseInput
{
    /// <summary>
    /// Id проекта, на который оставляют отклик.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Id вакансии.
    /// </summary>
    public long? VacancyId { get; set; }
}