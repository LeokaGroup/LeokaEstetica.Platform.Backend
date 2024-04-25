namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сопоставляется с таблицей панели модуля УП, документации УП.
/// </summary>
public class PanelEntity
{
    public PanelEntity(string itemName, string panelType, string items)
	{
        ItemName = itemName;
        PanelType = panelType;
        Items = items;
    }
    /// <summary>
    /// PK.
    /// </summary>
    public int PanelId { get; set; }

    /// <summary>
    /// Название пункта хидера.
    /// </summary>
    public string ItemName { get; set; }

    /// <summary>
    /// Путь (ссылка).
    /// </summary>
    public string? ItemUrl { get; set; }

    /// <summary>
    /// Порядковый номер элемента меню.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Тип элемента панели. Например, dropdown - выпадающий список.
    /// </summary>
    public string PanelType { get; set; }

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
}