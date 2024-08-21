using System.Globalization;
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
    
    /// <summary>
    /// Отображаемая дата.
    /// </summary>
    public string DisplayDateCreated => DateCreated.ToString("d", CultureInfo.GetCultureInfo("ru"));

    /// <summary>
    /// Признак проекта на модерации.
    /// </summary>
    public bool IsModeration { get; set; }

    /// <summary>
    /// Признак проекта в архиве.
    /// </summary>
    public bool IsArchived { get; set; }

    /// <summary>
    /// Кол-во записей (без учета пагинации).
    /// </summary>
    public long Total { get; set; }

    /// <summary>
    /// Id проекта в каталоге.
    /// </summary>
    public long CatalogProjectId { get; set; }
}