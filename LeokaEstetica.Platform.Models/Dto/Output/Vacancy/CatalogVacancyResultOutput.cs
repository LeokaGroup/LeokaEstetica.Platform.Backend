using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

/// <summary>
/// Класс выходной модели списка каталога вакансий.
/// </summary>
public class CatalogVacancyResultOutput : IFrontError
{
    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<string> Errors { get; set; }

    /// <summary>
    /// Список вакансий в каталоге.
    /// </summary>
    public IEnumerable<CatalogVacancyOutput> CatalogVacancies { get; set; }
}