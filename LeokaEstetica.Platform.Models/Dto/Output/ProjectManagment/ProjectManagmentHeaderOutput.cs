namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели хидера модуля УП (управление проектами).
/// </summary>
public class ProjectManagmentHeaderOutput
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
    /// Тип элемента хидера. Например, dropdown - выпадающий список.
    /// </summary>
    public string HeaderType { get; set; }
    
    /// <summary>
    /// Элементы пункта хидера. Содержат вложенные элементы пункта хидера, которые могут содержать также вложенные элементы.
    /// Вложенные элементы в формате jsonb.
    /// </summary>
    public string Items { get; set; }

    /// <summary>
    /// Признак наличие вложенных элементов пункта хидера.
    /// </summary>
    public bool HasItems { get; set; }

    /// <summary>
    /// Признак неактивности пункта.
    /// </summary>
    public bool IsDisabled { get; set; }
    
    /// <summary>
    /// Тип контрола (кнопка, выпадающий список и тд).
    /// </summary>
    public string ControlType { get; set; }

    /// <summary>
    /// Назначение (фильтры и тд).
    /// </summary>
    public string Destination { get; set; }
    
    /// <summary>
    /// Порядковый номер элемента меню.
    /// </summary>
    public int Position { get; set; }
}