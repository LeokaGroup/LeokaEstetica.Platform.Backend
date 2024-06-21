using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;

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
    Task<IEnumerable<WikiTreeItem>?> GetFolderItemsAsync(long projectId);

    /// <summary>
    /// Метод получает элементы страниц дерева.
    /// </summary>
    /// <param name="folderIds">Список Id папок.</param>
    /// <param name="treeIds">Список Id деревьев.</param>
    /// <returns>Список элементов страниц.</returns>
    Task<IEnumerable<WikiTreeItem>?> GetPageItemsAsync(IEnumerable<long> folderIds, IEnumerable<long> treeIds);

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
    Task<IEnumerable<WikiTreeItem>?> GetFolderStructureAsync(long projectId, long folderId);
    
    /// <summary>
    /// Метод получает содержимое страницы.
    /// </summary>
    /// <param name="pageId">Id страницы.</param>
    /// <returns>Содержимое страницы.</returns>
    Task<WikiTreeItem?> GetTreeItemPageAsync(long pageId);
    
    /// <summary>
    /// Метод изменяет название папки.
    /// </summary>
    /// <param name="folderName">Новое название папки.</param>
    /// <param name="folderId">Id папки.</param>
    Task UpdateFolderNameAsync(string? folderName, long folderId);
    
    /// <summary>
    /// Метод изменяет название страницы папки.
    /// </summary>
    /// <param name="pageName">Название страницы папки.</param>
    /// <param name="pageId">Id страницы.</param>
    Task UpdateFolderPageNameAsync(string? pageName, long pageId);
    
    /// <summary>
    /// Метод изменяет название страницы папки.
    /// </summary>
    /// <param name="pageDescription">Описание страницы папки.</param>
    /// <param name="pageId">Id страницы.</param>
    Task UpdateFolderPageDescriptionAsync(string? pageDescription, long pageId);

    /// <summary>
    /// Метод получает данные папки по ее Id.
    /// </summary>
    /// <param name="folderId">Id папки.</param>
    /// <returns>Данные папки.</returns>
    Task<WikiTreeItem?> GetFolderByFolderIdAsync(long folderId);

    /// <summary>
    /// Метод получает элементы контекстного меню.
    /// </summary>
    /// <param name="projectId">Id проекта, если передан.</param>
    /// <param name="pageId">Id страницы, если передан.</param>
    /// <returns>Элементы контекстного меню.</returns>
    Task<IEnumerable<WikiContextMenuOutput>> GetContextMenuAsync(long? projectId = null, long? pageId = null);
    
    /// <summary>
    /// Метод создает папку.
    /// </summary>
    /// <param name="parentId">Id родителя, если передали (родительская папка).</param>
    /// <param name="folderName">Название папки.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="treeId">Id дерева.</param>
    Task CreateFolderAsync(long? parentId, string? folderName, long userId, long treeId);
}