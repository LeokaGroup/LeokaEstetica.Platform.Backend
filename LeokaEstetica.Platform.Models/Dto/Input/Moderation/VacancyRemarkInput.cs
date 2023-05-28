using LeokaEstetica.Platform.Models.Dto.Base.Moderation.Input;

namespace LeokaEstetica.Platform.Models.Dto.Input.Moderation;

/// <summary>
/// Класс входной модели замечаний вакансии.
/// </summary>
public class VacancyRemarkInput : BaseRemarkInput
{
    /// <summary>
    /// Id вакансии.
    /// </summary>
    public long VacancyId { get; set; }
}