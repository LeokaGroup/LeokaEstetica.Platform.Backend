using LeokaEstetica.Platform.Models.Dto.Input.Project;

namespace LeokaEstetica.Platform.Controllers.ModelsValidation.Project;

/// <summary>
/// Класс валидации создания вакансии проекта.
/// </summary>
public class CreateProjectVacancyValidationModel : CreateProjectVacancyInput
{
    /// <summary>
    /// Аккаунт.
    /// </summary>
    public string Account { get; set; }
}