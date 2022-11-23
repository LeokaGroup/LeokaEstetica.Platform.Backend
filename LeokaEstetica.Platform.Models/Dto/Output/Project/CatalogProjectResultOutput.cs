namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели результата каталога проектов.
/// </summary>
public class CatalogProjectResultOutput
{
    /// <summary>
    /// Список проектов.
    /// </summary>
    public IEnumerable<CatalogProjectOutput> CatalogProjects { get; set; }

    /// <summary>
    /// Кол-во проектов.
    /// </summary>
    public int Total => CatalogProjects.Count();
}