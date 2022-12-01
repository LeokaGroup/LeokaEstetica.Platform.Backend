using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;

namespace LeokaEstetica.Platform.Models.Dto.Input.Project;

/// <summary>
/// Класс входной модели создания вакансии проекта.
/// </summary>
public class CreateProjectVacancyInput : CreateVacancyBase
{
    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }
}