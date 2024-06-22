namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели создания папки Wiki проекта.
/// </summary>
public class CreateWikiFolderInput
{
    /// <summary>
    /// Id родителя, если передали (родительская папка).
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// Название папки.
    /// </summary>
    public string? FolderName { get; set; }

    /// <summary>
    /// Id дерева.
    /// </summary>
    public long WikiTreeId { get; set; }
}