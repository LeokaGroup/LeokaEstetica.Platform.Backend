using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.ProjectManagement.Output;

/// <summary>
/// Базовый класс настроек Scrum.
/// </summary>
public class BaseScrumSetting
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="name">Название настройки.</param>
    public BaseScrumSetting(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Название настройки.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Системное название настройки.
    /// </summary>
    [JsonProperty("key")]
    public string? SysName { get; set; }

    /// <summary>
    /// Текст подсказки к настройке.
    /// </summary>
    [JsonProperty("label")]
    public string? Tooltip { get; set; }

    /// <summary>
    /// Признак выбранной пользователем настройки.
    /// </summary>
    [JsonProperty("checked")]
    public bool Selected { get; set; }

    /// <summary>
    /// Признак заблокированной настройки.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }
}