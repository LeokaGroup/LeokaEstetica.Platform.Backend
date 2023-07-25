using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Base.Moderation.Output;

/// <summary>
/// Базовый класс выходной модели замечаний.
/// </summary>
public class BaseGetRemarkOutput
{
    /// <summary>
    /// Текст замечания.
    /// </summary>
    [JsonProperty("detail")]
    protected string RemarkText { get; set; }

    /// <summary>
    /// Уровень замечания.
    /// </summary>
    protected string Severity { get; set; }
}