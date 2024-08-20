using LeokaEstetica.Platform.Models.Enums;

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
    /// Фильтр по дате.
    /// </summary>
    public string? Date { get; set; }

    /// <summary>
    /// Признак проектов с наличием вакансий.
    /// </summary>
    public bool IsAnyVacancies { get; set; }

    /// <summary>
    /// Фильтр стадий проекта (может содержать несколько значений).
    /// </summary>
    public string? StageValues { get; set; }

    /// <summary>
    /// Список стадий проекта.
    /// </summary>
    public List<FilterProjectStageTypeEnum>? ProjectStages { get; set; }
}