using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Vacancy;

namespace LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

/// <summary>
/// Класс выходной модели вакансии.
/// </summary>
public class VacancyOutput : VacancyRemarkResult, IFrontError
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
    
    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure> Errors { get; set; }

    /// <summary>
    /// Признак отображения кнопки удаления вакансии.
    /// </summary>
    public bool IsVisibleDeleteButton { get; set; }
    
    /// <summary>
    /// Признак отображения кнопки сохранения вакансии.
    /// </summary>
    public bool IsVisibleSaveButton { get; set; }
    
    /// <summary>
    /// Признак отображения кнопки изменения вакансии.
    /// </summary>
    public bool IsVisibleEditButton { get; set; }

    /// <summary>
    /// Статус вакансии.
    /// </summary>
    public string VacancyStatusName { get; set; }
    
    /// <summary>
    /// Требования.
    /// </summary>
    public string Demands { get; set; }

    /// <summary>
    /// Условия.
    /// </summary>
    public string Conditions { get; set; }

    /// <summary>
    /// Признак отображения кнопки добавления вакансии в архив.
    /// </summary>
    public bool IsVisibleActionAddVacancyArchive { get; set; }
    
    /// <summary>
    /// Признак доступа.
    /// </summary>
    public bool IsAccess { get; set; }
    
    /// <summary>
    /// Успешное ли сохранение.
    /// </summary>
    public bool IsSuccess { get; set; }
}