namespace LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

/// <summary>
/// Класс выходной модели с результатами для меню вакансий.
/// </summary>
public class VacancyMenuItemsResultOutput
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