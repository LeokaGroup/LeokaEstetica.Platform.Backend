using System.Text.Json.Serialization;

namespace LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

/// <summary>
/// Класс выходной модели замечаний вакансии.
/// </summary>
public class GetVacancyRemarkOutput
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
    
    public GetVacancyRemarkOutput()
    {
        Severity = "warn";
    }
}