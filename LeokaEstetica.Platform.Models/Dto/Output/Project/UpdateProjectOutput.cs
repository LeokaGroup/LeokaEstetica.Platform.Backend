using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели обновления проекта.
/// </summary>
public class UpdateProjectOutput : ProjectOutput, IFrontError
{
    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<string> Errors { get; set; }
}