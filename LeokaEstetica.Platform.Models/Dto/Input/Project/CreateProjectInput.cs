using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Input.Project;

/// <summary>
/// Класс входной модели для создания нового проекта.
/// </summary>
public class CreateProjectInput : IFrontError
{
    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<string> Errors { get; set; } = new();
}