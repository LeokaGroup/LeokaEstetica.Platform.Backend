using LeokaEstetica.Platform.Models.Dto.Input.Project;

namespace LeokaEstetica.Platform.Controllers.ModelsValidation.Project;

/// <summary>
/// Класс валидации создания вакансии проекта.
/// </summary>
public class CreateProjectVacancyValidationModel : CreateProjectVacancyInput
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyText">Описание вакансии.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    public CreateProjectVacancyValidationModel(string vacancyName, string vacancyText, long vacancyId, long projectId,
        long userId) : base(vacancyName, vacancyText, vacancyId, projectId, userId)
    {
    }
}