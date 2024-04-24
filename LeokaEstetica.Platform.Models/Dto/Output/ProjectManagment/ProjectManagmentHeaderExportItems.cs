namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс списка вложенных элементов хидера модуля УП (экспорт).
/// </summary>
public class ProjectManagmentHeaderExportItems
{
    /// <summary>
    /// Id (системный идентификатор в БД).
    /// </summary>
    public string Id { get; set; }
    
    /// <summary>
    /// Список элементов.
    /// </summary>
    public List<ExportItems> Items { get; set; }
    
    /// <summary>
    /// Признак элемента футера меню.
    /// </summary>
    public bool IsFooterItem { get; set; }
}

public class ExportItems
{
    /// <summary>
    /// Id (системный идентификатор в БД).
    /// </summary>
    public string Id { get; set; }
    
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
    public bool Disabled { get; set; }
    
    /// <summary>
    /// Вложенные элементы в формате jsonb.
    /// </summary>
    public string Items { get; set; }
    
    /// <summary>
    /// Признак элемента футера меню.
    /// </summary>
    public bool IsFooterItem { get; set; }
}