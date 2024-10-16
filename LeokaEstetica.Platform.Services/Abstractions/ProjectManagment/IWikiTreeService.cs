﻿using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;

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
    Task<IEnumerable<WikiTreeItem>> GetTreeAsync(long projectId);

    /// <summary>
    /// Метод получает папку (и ее структуру - вложенные папки и страницы).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="folderId">Id папки.</param>
    /// <returns>Структура папки.</returns>
    Task<IEnumerable<WikiTreeItem>> GetTreeItemFolderAsync(long projectId, long folderId);

    /// <summary>
    /// Метод получает содержимое страницы.
    /// </summary>
    /// <param name="pageId">Id страницы.</param>
    /// <returns>Содержимое страницы.</returns>
    Task<WikiTreeItem> GetTreeItemPageAsync(long pageId);

    /// <summary>
    /// Метод изменяет название папки.
    /// </summary>
    /// <param name="folderName">Название папки.</param>
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
    /// Метод создает папку.
    /// </summary>
    /// <param name="parentId">Id родителя, если передали (родительская папка).</param>
    /// <param name="folderName">Название папки.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="treeId">Id дерева. Может быть не заполненно,
    /// <param name="projectId">Id проекта.
    /// в таком кейсе создаем папку как родителя или отдельную страницу.</param>
    Task CreateFolderAsync(long? parentId, string? folderName, string account, long? treeId, long projectId);

    /// <summary>
    /// Метод создает страницу.
    /// </summary>
    /// <param name="parentId">Id родителя, если передали (родительская папка).</param>
    /// <param name="pageName">Название страницы.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="treeId">Id дерева.</param>
    /// <param name="projectId">Id проекта.
    Task CreatePageAsync(long? parentId, string? pageName, string account, long? treeId, long projectId);

    /// <summary>
    /// Метод удаляет папку.
    /// Удаляет все дочерние папки и страницы у этой папки.
    /// </summary>
    /// <param name="folderId">Id папки.</param>
    /// <param name="isApprove">Признак согласия пользователя на удаление дочерних элементов.</param>
    /// <returns>Выходная модель.</returns>
    Task<RemoveFolderResponseOutput> RemoveFolderAsync(long folderId, bool isApprove);

    /// <summary>
    /// Метод удаляет страницу.
    /// </summary>
    /// <param name="pageId">Id страницы.</param>
    Task RemovePageAsync(long pageId);
}