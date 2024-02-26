namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели тегов проекта.
/// </summary>
public class ProjectTagInput
{
    /// <summary>
    /// Название тега.
    /// </summary>
    public string TagName { get; set; }

    /// <summary>
    /// Описание метки (тега).
    /// </summary>
    public string TagDescription { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }
}