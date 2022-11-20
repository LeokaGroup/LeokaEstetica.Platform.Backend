namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели проекта.
/// </summary>
public class ProjectOutput
{
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

    /// <summary>
    /// Дата создания проекта.
    /// </summary>
    public DateTime DateCreated { get; set; }
}