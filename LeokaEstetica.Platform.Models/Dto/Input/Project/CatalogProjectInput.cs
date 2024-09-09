namespace LeokaEstetica.Platform.Models.Dto.Input.Project;

/// <summary>
/// Класс входной модели каталога проектов.
/// </summary>
public class CatalogProjectInput
{
    /// <summary>
    /// Id последней записи на фронте. Применяется дял пагинации.
    /// Если NULL, то отдаем 1 страницу каталога.
    /// </summary>
    public long? LastId { get; set; }

    /// <summary>
    /// TODO: Будет передаваться с фронта, не убирать пока.
    /// Кол-во записей в выборке.
    /// По дефолту 20.
    /// </summary>
    public short PaginationRows { get; set; }

    /// <summary>
    /// Класс входной модели фильтрации проектов.
    /// </summary>
    public FilterProjectInput? Filters { get; set; }

    /// <summary>
	/// Строка для поискового запроса проектов
	/// </summary>
	public string? SearchText { get; set; }

    /// <summary>
    /// Признак использования пагинации.
    /// </summary>
    public bool IsPagination { get; set; }
}