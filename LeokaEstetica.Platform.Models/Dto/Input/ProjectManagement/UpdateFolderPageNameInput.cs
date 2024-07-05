namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели изменения названия страницы папки Wiki проекта.
/// </summary>
public class UpdateFolderPageNameInput
{
    /// <summary>
    /// Id страницы.
    /// </summary>
    public long PageId { get; set; }

    /// <summary>
    /// Название страницы.
    /// </summary>
    public string? PageName { get; set; }
}