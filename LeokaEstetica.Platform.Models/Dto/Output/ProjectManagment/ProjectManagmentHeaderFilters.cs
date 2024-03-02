using Newtonsoft.Json;

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
    public bool Disabled { get; set; }
    
    /// <summary>
    /// Уникальный идентификатор (обычно системное название).
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Id { get; set; }
}