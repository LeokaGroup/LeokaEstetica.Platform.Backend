namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс списка вложенных элементов хидера модуля УП (настройки).
/// </summary>
public class ProjectManagmentHeaderSettings
{
    /// <summary>
    /// Список элементов.
    /// </summary>
    public List<Setting> Items { get; set; }
}

public class Setting
{
    /// <summary>
    /// Название пункта хидера.
    /// </summary>
    public string ItemName { get; set; }
    
    /// <summary>
    /// Порядковый номер элемента меню.
    /// </summary>
    public int Position { get; set; }
}