using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;

namespace LeokaEstetica.Platform.Models.Dto.Input.Project;

/// <summary>
/// Класс входной модели создания вакансии проекта.
/// </summary>
public class CreateProjectVacancyInput : VacancyInput
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyText">Описание вакансии.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    public CreateProjectVacancyInput(string vacancyName, string vacancyText, long vacancyId, long projectId,
        long userId) : base(vacancyName, vacancyText, vacancyId, projectId, userId)
    {
    }
}