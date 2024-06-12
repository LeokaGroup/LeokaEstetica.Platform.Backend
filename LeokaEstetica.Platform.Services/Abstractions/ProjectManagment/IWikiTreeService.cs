using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;

namespace LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;

/// <summary>
/// Абстракция сервиса дерева Wiki модуля УП.
/// </summary>
public interface IWikiTreeService
{
    /// <summary>
    /// Метод получает дерево.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Дерево с вложенными элементами.</returns>
    Task<IEnumerable<WikiTreeFolderItem>> GetTreeAsync(long projectId);

    /// <summary>
    /// Метод получает папку (и ее структуру - вложенные папки и страницы).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="folderId">Id папки.</param>
    /// <returns>Структура папки.</returns>
    Task<IEnumerable<WikiTreeFolderItem>> GetTreeItemFolderAsync(long projectId, long folderId);

    /// <summary>
    /// Метод получает содержимое страницы.
    /// </summary>
    /// <param name="pageId">Id страницы.</param>
    /// <returns>Содержимое страницы.</returns>
    Task<WikiTreePageItem> GetTreeItemPageAsync(long pageId);
}