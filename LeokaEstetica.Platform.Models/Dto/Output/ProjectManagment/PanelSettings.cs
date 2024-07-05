using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс списка вложенных элементов панели модуля УП (настройки).
/// </summary>
public class PanelSettings
{
    /// <summary>
    /// Список элементов.
    /// </summary>
    public List<Settings> Items { get; set; }
    
    /// <summary>
    /// Уникальный идентификатор (обычно системное название).
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Id { get; set; }
    
    /// <summary>
    /// Признак неактивности пункта.
    /// </summary>
    public bool Disabled { get; set; }
    
    /// <summary>
    /// Признак элемента футера меню.
    /// </summary>
    public bool IsFooterItem { get; set; }
    
    /// <summary>
    /// Признак отображения элемента.
    /// </summary>
    public bool Visible { get; set; }
}

public class Settings
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
    public bool Disabled { get; set; }
    
    /// <summary>
    /// Уникальный идентификатор (обычно системное название).
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Id { get; set; }
    
    /// <summary>
    /// Признак элемента футера меню.
    /// </summary>
    public bool IsFooterItem { get; set; }
    
    /// <summary>
    /// Признак отображения элемента.
    /// </summary>
    public bool Visible { get; set; }
}