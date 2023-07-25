using LeokaEstetica.Platform.Models.Dto.Input.Project;

namespace LeokaEstetica.Platform.Controllers.ModelsValidation.Project;

/// <summary>
/// Класс валидации создания проекта.
/// </summary>
public class CreateProjectValidationModel : CreateProjectInput
{
    /// <summary>
    /// Аккаунт пользователя.
    /// </summary>
    public string Account { get; set; }
}