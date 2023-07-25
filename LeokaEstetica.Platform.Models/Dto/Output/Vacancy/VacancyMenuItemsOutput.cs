namespace LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

/// <summary>
/// Класс выходной модели для списков меню вакансий.
/// </summary>
public class VacancyMenuItemsOutput
{
    /// <summary>
    /// Название пункта.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Список элементов.
    /// </summary>
    public List<VacancyItems> Items { get; set; } = new();
    
    /// <summary>
    /// Системное название.
    /// </summary>
    public string SysName { get; set; }

    /// <summary>
    /// Путь.
    /// </summary>
    public string Url { get; set; }
}

/// <summary>
/// Класс вложенных элементов списка меню вакансий.
/// </summary>
public class VacancyItems
{
    /// <summary>
    /// Название.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Системное название.
    /// </summary>
    public string SysName { get; set; }

    /// <summary>
    /// Путь.
    /// </summary>
    public string Url { get; set; }
}