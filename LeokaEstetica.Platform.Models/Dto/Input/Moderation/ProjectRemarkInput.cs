namespace LeokaEstetica.Platform.Models.Dto.Input.Moderation;

/// <summary>
/// Класс входной модели замечаний проекта.
/// </summary>
public class ProjectRemarkInput
{
    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Название поля.
    /// </summary>
    public string FieldName { get; set; }

    /// <summary>
    /// Текст замечания.
    /// </summary>
    public string RemarkText { get; set; }

    /// <summary>
    /// Русское название замечания.
    /// </summary>
    public string RussianName { get; set; }
}