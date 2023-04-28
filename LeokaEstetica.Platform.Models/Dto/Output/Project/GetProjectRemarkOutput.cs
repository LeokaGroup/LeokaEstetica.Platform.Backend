using System.Text.Json.Serialization;

namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели замечаний проекта.
/// </summary>
public class GetProjectRemarkOutput
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
    
    public GetProjectRemarkOutput()
    {
        Severity = "warn";
    }
}