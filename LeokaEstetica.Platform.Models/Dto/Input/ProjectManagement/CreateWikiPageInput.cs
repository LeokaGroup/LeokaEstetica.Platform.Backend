namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели создания страницы Wiki проекта.
/// </summary>
public class CreateWikiPageInput
{
    /// <summary>
    /// Id родителя, если передали (родительская папка).
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// Название страницы.
    /// </summary>
    public string? PageName { get; set; }

    /// <summary>
    /// Id дерева.
    /// </summary>
    public long WikiTreeId { get; set; }
}