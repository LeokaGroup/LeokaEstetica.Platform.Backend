using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;

/// <summary>
/// Класс выходной модели контекстного меню Wiki.
/// </summary>
public class WikiContextMenuOutput
{
    /// <summary>
    /// Id элемента меню.
    /// </summary>
    public int MenuId { get; set; }

    /// <summary>
    /// Название элемента меню.
    /// </summary>
    [JsonProperty("label")]
    public string? ItemName { get; set; }

    /// <summary>
    /// Иконка элемента меню.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Системное название элемента меню.
    /// </summary>
    [JsonProperty("key")]
    public string? ItemSysName { get; set; }
}