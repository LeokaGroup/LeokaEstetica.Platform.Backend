using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс результата выходной модели панели модуля УП (управление проектами) и модуля документации УП.
/// </summary>
public class PanelResult
{
    /// <summary>
    /// Название.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Уникальный идентификатор (обычно системное название).
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Id { get; set; }
    
    /// <summary>
    /// Вложенные элементы.
    /// </summary>
    public IEnumerable<Panel> Items { get; set; }
    
    /// <summary>
    /// Признак неактивности пункта меню.
    /// </summary>
    public bool Disabled { get; set; }
    
    /// <summary>
    /// Признак элемента футера меню.
    /// </summary>
    public bool IsFooterItem { get; set; }
    
    /// <summary>
    /// Команда (используется фронтом для js-логики).
    /// </summary>
    public string? Command { get; set; }
}

public class Panel
{
    /// <summary>
    /// Название.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Вложенные элементы.
    /// </summary>
    public IEnumerable<PanelItems> Items { get; set; }
    
    /// <summary>
    /// Уникальный идентификатор (обычно системное название).
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Id { get; set; }

    /// <summary>
    /// Признак неактивности пункта меню.
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
    
    /// <summary>
    /// Команда (используется фронтом для js-логики).
    /// </summary>
    public string? Command { get; set; }
}

public class PanelItems
{
    /// <summary>
    /// Название.
    /// </summary>
    public string Label { get; set; }
    
    /// <summary>
    /// Уникальный идентификатор (обычно системное название).
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Id { get; set; }
    
    /// <summary>
    /// Признак неактивности пункта меню.
    /// </summary>
    public bool Disabled { get; set; }
    
    /// <summary>
    /// Признак элемента футера меню.
    /// </summary>
    public bool IsFooterItem { get; set; }
}