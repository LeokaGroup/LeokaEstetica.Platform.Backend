namespace LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

/// <summary>
/// Класс выходной модели списка каталога вакансий.
/// </summary>
public class CatalogVacancyResultOutput
{
    /// <summary>
    /// Список вакансий в каталоге.
    /// </summary>
    public IEnumerable<CatalogVacancyOutput>? CatalogVacancies { get; set; }

    /// <summary>
    /// Кол-во найденных записей (несмотря на пагинацию).
    /// </summary>
    public long Total { get; set; }

    /// <summary>
    /// Id последней записи выборки (подсказка фронту для пагинации).
    /// </summary>
    public long? LastId { get; set; }
}