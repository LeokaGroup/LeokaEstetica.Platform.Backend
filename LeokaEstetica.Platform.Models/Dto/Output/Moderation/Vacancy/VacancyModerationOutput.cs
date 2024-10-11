namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Vacancy;

/// <summary>
/// Класс выходной модели вакансий для модерации.
/// </summary>
public class VacancyModerationOutput
{
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
    public string DateCreated { get; set; }
    
    /// <summary>
    /// Дата модерации вакансии.
    /// </summary>
    public string DateModeration { get; set; }

    /// <summary>
    /// Оплата у вакансии.
    /// Если не указано, то выводится текст "Не указана".
    /// </summary>
    public string Payment { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Название статуса модерации.
    /// </summary>
    public string ModerationStatusName { get; set; }

	/// <summary>
	/// Статус оплаты вакансии
	/// </summary>
	public bool IsPaymentCompleted { get; set; }
}