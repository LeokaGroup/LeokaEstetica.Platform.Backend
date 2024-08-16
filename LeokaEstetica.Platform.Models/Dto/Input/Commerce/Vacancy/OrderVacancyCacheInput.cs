using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;

namespace LeokaEstetica.Platform.Models.Dto.Input.Commerce.Vacancy;

/// <summary>
/// Класс входной модели заказа вакансии в кэше.
/// </summary>
public class OrderVacancyCacheInput : VacancyInput
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyText">Описание вакансии.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    public OrderVacancyCacheInput(string vacancyName,
        string vacancyText,
        long? vacancyId,
        long projectId,
        long? userId)
        : base(vacancyName, vacancyText, vacancyId, projectId, userId)
    {
    }
}