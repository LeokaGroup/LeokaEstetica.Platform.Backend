using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели каталога проектов.
/// </summary>
public class CatalogProjectOutput
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
    /// Дата создания проекта.
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Признак наличия вакансий у проекта.
    /// </summary>
    public bool HasVacancies { get; set; }

    /// <summary>
    /// Системное название проекта.
    /// </summary>
    public string ProjectStageSysName { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    [JsonIgnore]
    public long UserId { get; set; }

    /// <summary>
    /// Признак выделения цветом.
    /// </summary>
    public bool IsSelectedColor { get; set; }
    
    /// <summary>
    /// Цвет тега.
    /// </summary>
    public string TagColor { get; set; }

    /// <summary>
    /// Значение тега.
    /// </summary>
    public string TagValue { get; set; }
}