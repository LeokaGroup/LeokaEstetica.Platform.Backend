using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Models.Dto.Input.Project;

/// <summary>
/// Класс входной модели фильтрации проектов.
/// </summary>
public class FilterProjectInput
{
    /// <summary>
    /// Фильтр по дате.
    /// </summary>
    public string Date { get; set; }

    /// <summary>
    /// Признак проектов с наличием вакансий.
    /// </summary>
    public bool IsAnyVacancies { get; set; }

    /// <summary>
    /// Фильтр стадий проекта (может содержать несколько значений).
    /// </summary>
    public string StageValues { get; set; }

    /// <summary>
    /// Список стадий проекта.
    /// </summary>
    public List<FilterProjectStageTypeEnum> ProjectStages { get; set; }
}