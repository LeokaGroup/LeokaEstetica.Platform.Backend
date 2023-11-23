namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс списка вложенных элементов хидера модуля УП (стратегии представления).
/// </summary>
public class ProjectManagmentHeaderStrategyItems
{
    /// <summary>
    /// Список элементов.
    /// </summary>
    public List<StrategyItems> Items { get; set; }
}

public class StrategyItems
{
    /// <summary>
    /// Название пункта хидера.
    /// </summary>
    public string ItemName { get; set; }

    /// <summary>
    /// Путь (ссылка).
    /// </summary>
    public string ItemUrl { get; set; }
    
    /// <summary>
    /// Порядковый номер элемента меню.
    /// </summary>
    public int Position { get; set; }
    
    /// <summary>
    /// Признак наличие вложенных элементов пункта хидера.
    /// </summary>
    public bool HasItems { get; set; }
    
    /// <summary>
    /// Признак неактивности пункта.
    /// </summary>
    public bool IsDisabled { get; set; }
    
    /// <summary>
    /// Вложенные элементы в формате jsonb.
    /// </summary>
    public string Items { get; set; }
}