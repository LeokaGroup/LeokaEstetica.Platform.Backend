namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели изменения названия названия/описания страницы папки Wiki проекта.
/// </summary>
public class UpdateFolderPageNameDescriptionInput
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