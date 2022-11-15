using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

/// <summary>
/// Класс выходной модели создания вакансии.
/// </summary>
public class CreateVacancyOutput : IFrontError
{
    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<string> Errors { get; set; }
    
    /// <summary>
    /// PK.
    /// </summary>
    public long VacancyId { get; set; }

    /// <summary>
    /// Название вакансии.
    /// </summary>
    public string VacancyName { get; set; }

    /// <summary>
    /// Описание вакансии.
    /// </summary>
    public string VacancyText { get; set; }

    /// <summary>
    /// Опыт работы.
    /// Указывается текстом в виде: Требуемый опыт работы: 1–3 года.
    /// Если не указано, то выводится "Без опыта работы".
    /// </summary>
    public string WorkExperience { get; set; }

    /// <summary>
    /// Занятость у вакансии.
    /// Например: Полная занятость, удаленная работа.
    /// Разделяется сепаратором через запятую.
    /// Если не указано, то выводится текст "Занятость не указана".
    /// </summary>
    public string Employment { get; set; }

    /// <summary>
    /// Дата создания вакансии.
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Оплата у вакансии.
    /// Если не указано, то выводится текст "Не указана".
    /// </summary>
    public string Payment { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }
}