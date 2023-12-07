namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс списка вложенных элементов хидера модуля УП (фильтры).
/// </summary>
public class ProjectManagmentHeaderFilters
{
    /// <summary>
    /// Список элементов.
    /// </summary>
    public List<Filters> Items { get; set; }
}

public class Filters
{
    /// <summary>
    /// Название пункта хидера.
    /// </summary>
    public string ItemName { get; set; }
    
    /// <summary>
    /// Порядковый номер элемента меню.
    /// </summary>
    public int Position { get; set; }
    
    /// <summary>
    /// Признак неактивности пункта.
    /// </summary>
    public bool IsDisabled { get; set; }
}