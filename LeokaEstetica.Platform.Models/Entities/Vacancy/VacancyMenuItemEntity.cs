using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

namespace LeokaEstetica.Platform.Models.Entities.Vacancy;

/// <summary>
/// Класс сопоставляется с таблицей меню вакансий (т.е это левое меню вакансий). Vacancies.VacancyMenuItems. 
/// </summary>
public class VacancyMenuItemEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int VacancyMenuItemId { get; set; }

    /// <summary>
    /// Элементы меню вакансий.
    /// </summary>
    public List<VacancyMenuItemsOutput> VacancyMenuItems { get; set; }
}