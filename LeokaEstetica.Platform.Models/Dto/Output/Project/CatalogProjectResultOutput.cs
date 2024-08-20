namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели результата каталога проектов.
/// </summary>
public class CatalogProjectResultOutput
{
    /// <summary>
    /// Список проектов.
    /// </summary>
    public IEnumerable<CatalogProjectOutput>? CatalogProjects { get; set; }

    /// <summary>
    /// Кол-во найденных записей (несмотря на пагинацию).
    /// </summary>
    public long Total { get; set; }

    /// <summary>
    /// Id последней записи выборки (подсказка фронту для пагинации).
    /// </summary>
    public long? LastId { get; set; }
}