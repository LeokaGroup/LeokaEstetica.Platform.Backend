namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сопоставляется с таблицей хидера модуля УП (управление проектами).
/// </summary>
public class ProjectManagmentHeaderEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int HeaderId { get; set; }

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
}