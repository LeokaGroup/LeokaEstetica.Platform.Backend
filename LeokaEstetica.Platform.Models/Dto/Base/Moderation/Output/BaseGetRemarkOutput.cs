using System.Text.Json.Serialization;

namespace LeokaEstetica.Platform.Models.Dto.Base.Moderation.Output;

/// <summary>
/// Базовый класс выходной модели замечаний.
/// </summary>
public class BaseGetRemarkOutput
{
    /// <summary>
    /// Текст замечания.
    /// </summary>
    [JsonPropertyName("detail")]
    public string RemarkText { get; set; }

    /// <summary>
    /// Уровень замечания.
    /// </summary>
    public string Severity { get; set; }
}