using LeokaEstetica.Platform.Access.Enums;

namespace LeokaEstetica.Platform.Controllers.ModelsValidation.Project;

/// <summary>
/// Класс модели валидации получения проекта.
/// </summary>
public class GetProjectValidationModel
{
    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Режим. Чтение или изменение.
    /// </summary>
    public ModeEnum Mode { get; set; }
}