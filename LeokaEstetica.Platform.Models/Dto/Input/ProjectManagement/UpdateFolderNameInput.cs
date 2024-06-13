namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели имзенения названия папки Wiki проекта.
/// </summary>
public class UpdateFolderNameInput
{
    /// <summary>
    /// Id папки.
    /// </summary>
    public long FolderId { get; set; }

    /// <summary>
    /// Новое название папки.
    /// </summary>
    public string? FolderName { get; set; }
}