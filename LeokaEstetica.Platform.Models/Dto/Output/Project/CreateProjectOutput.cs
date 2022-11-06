using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели 
/// </summary>
public class CreateProjectOutput : IFrontError
{
    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<string> Errors { get; set; } = new();
    
    /// <summary>
    /// Название проекта.
    /// </summary>
    public string ProjectName { get; set; }

    /// <summary>
    /// Описание проекта.
    /// </summary>
    public string ProjectDetails { get; set; }

    /// <summary>
    /// Изображение проекта.
    /// </summary>
    public string ProjectIcon { get; set; }
    
    /// <summary>
    /// PK.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Успешное ли сохранение.
    /// </summary>
    public bool IsSuccess { get; set; }
}