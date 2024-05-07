namespace LeokaEstetica.Platform.Models.Dto.Input.Vacancy;

/// <summary>
/// Класс входной модели вакансии.
/// </summary>
public class VacancyInput
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyText">Описание вакансии.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    public VacancyInput(string vacancyName, string vacancyText, long? vacancyId, long projectId, long? userId)
    {
        if (vacancyId.HasValue)
        {
            VacancyId = vacancyId.Value;
        }

        if (userId.HasValue)
        {
            UserId = userId.Value;
        }
        
        VacancyName = vacancyName;
        VacancyText = vacancyText;
        ProjectId = projectId;
    }

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
    public string? WorkExperience { get; set; }

    /// <summary>
    /// Занятость у вакансии.
    /// Например: Полная занятость, удаленная работа.
    /// Разделяется сепаратором через запятую.
    /// Если не указано, то выводится текст "Занятость не указана".
    /// </summary>
    public string? Employment { get; set; }

    /// <summary>
    /// Оплата у вакансии.
    /// Если не указано, то выводится текст "Не указана".
    /// </summary>
    public string? Payment { get; set; }

    /// <summary>
    /// Id вакансии.
    /// </summary>
    public long? VacancyId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Аккаунт.
    /// </summary>
    public string? Account { get; set; }

    /// <summary>
    /// Токен пользователя.
    /// </summary>
    public string? Token { get; set; }
    
    /// <summary>
    /// Требования.
    /// </summary>
    public string? Demands { get; set; }

    /// <summary>
    /// Условия.
    /// </summary>
    public string? Conditions { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long? UserId { get; set; }
}