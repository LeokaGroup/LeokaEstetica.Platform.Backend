using LeokaEstetica.Platform.Access.Enums;

namespace LeokaEstetica.Platform.Controllers.ModelsValidation.Vacancy;

/// <summary>
/// Класс модели валидации получения вакансии.
/// </summary>
public class VacancyValidationModel
{
    /// <summary>
    /// Id вакансии.
    /// </summary>
    public long VacancyId { get; set; }

    /// <summary>
    /// Режим. Чтение или изменение.
    /// </summary>
    public ModeEnum Mode { get; set; }
}