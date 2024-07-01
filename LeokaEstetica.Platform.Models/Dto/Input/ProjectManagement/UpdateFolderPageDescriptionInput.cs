namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели изменения описания страницы папки Wiki проекта.
/// </summary>
public class UpdateFolderPageDescriptionInput
{
    /// <summary>
    /// Id страницы.
    /// </summary>
    public long PageId { get; set; }

    /// <summary>
    /// Описание страницы.
    /// </summary>
    public string? PageDescription { get; set; }
}