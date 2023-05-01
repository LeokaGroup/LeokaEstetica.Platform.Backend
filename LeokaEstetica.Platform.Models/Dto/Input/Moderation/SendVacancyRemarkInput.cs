namespace LeokaEstetica.Platform.Models.Dto.Input.Moderation;

/// <summary>
/// Класс входной модели отправки замечаний вакансии.
/// </summary>
public class SendVacancyRemarkInput
{
    /// <summary>
    /// Id вакансии.
    /// </summary>
    public long VacancyId { get; set; }
}