using LeokaEstetica.Platform.Models.Dto.Base.Moderation.Output;

namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Vacancy;

/// <summary>
/// Класс выходной модели замечания вакансий.
/// </summary>
public class VacancyRemarkOutput : BaseRemarkOutput
{
    /// <summary>
    /// Id вакансии.
    /// </summary>
    public long VacancyId { get; set; }
}