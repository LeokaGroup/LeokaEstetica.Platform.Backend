using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;

namespace LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;

/// <summary>
/// Абстракция репозитория дерева Wiki модуля УП.
/// </summary>
public interface IWikiTreeRepository
{
    /// <summary>
    /// Метод получает элементы папок дерева.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список элементов папок дерева.</returns>
    Task<IEnumerable<WikiTreeFolderItem>?> GetFolderItemsAsync(long projectId);

    /// <summary>
    /// Метод получает элементы страниц дерева.
    /// </summary>
    /// <param name="folderIds">Список Id папок.</param>
    /// <param name="treeIds">Список Id деревьев.</param>
    /// <returns>Список элементов страниц.</returns>
    Task<IEnumerable<WikiTreePageItem>?> GetPageItemsAsync(IEnumerable<long> folderIds, IEnumerable<long> treeIds);

    /// <summary>
    /// Метод создает Wiki для проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectName">Название проекта.</param>
    Task CreateProjectWikiAsync(long projectId, long userId, string? projectName);

    /// <summary>
    /// Метод получает структуру папки.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="folderId">Id папки.</param>
    /// <returns>Структура папки. Вложенные папки и страницы.</returns>
    Task<IEnumerable<WikiTreeFolderItem>?> GetFolderStructureAsync(long projectId, long folderId);
    
    /// <summary>
    /// Метод получает содержимое страницы.
    /// </summary>
    /// <param name="pageId">Id страницы.</param>
    /// <returns>Содержимое страницы.</returns>
    Task<WikiTreePageItem?> GetTreeItemPageAsync(long pageId);
}