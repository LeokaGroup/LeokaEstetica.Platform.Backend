using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Models.Dto.Input.Vacancy;

/// <summary>
/// Класс входной модели фильтрации вакансий.
/// </summary>
public class FilterVacancyInput
{
    /// <summary>
    /// Тип фильтра по соответствиям.
    /// <seealso cref="FilterSalaryTypeEnum"/>.
    /// </summary>
    public string? Salary { get; set; }

    /// <summary>
    /// Фильтр оплаты.
    /// <seealso cref="FilterPayTypeEnum"/>.
    /// </summary>
    public string? Pay { get; set; }

    /// <summary>
    /// Фильтр опыта работы.
    /// <seealso cref="FilterExperienceTypeEnum"/>.
    /// </summary>
    public string? Experience { get; set; }

    /// <summary>
    /// Фильтр занятости (список значений).
    /// <seealso cref="FilterEmploymentTypeEnum"/>.
    /// </summary>
    public string? EmploymentsValues { get; set; }

    /// <summary>
    /// Список значений после парсинга списка знчений.
    /// </summary>
    public List<FilterEmploymentTypeEnum>? Employments { get; set; }
}