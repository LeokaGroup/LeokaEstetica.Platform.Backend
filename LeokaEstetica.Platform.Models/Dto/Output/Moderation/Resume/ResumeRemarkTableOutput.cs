namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Resume;

/// <summary>
/// Класс выходной модели для замечаний таблицы анкет.
/// </summary>
public class ResumeRemarkTableOutput
{
    /// <summary>
    /// Id анкеты.
    /// </summary>
    public long ProfileInfoId { get; set; }

    /// <summary>
    /// Название анкеты (ФИО).
    /// </summary>
    public string VacancyName { get; set; }
}