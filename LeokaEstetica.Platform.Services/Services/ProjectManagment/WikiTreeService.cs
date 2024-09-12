﻿using System.Runtime.CompilerServices;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Services.Services.ProjectManagment;

/// <summary>
/// Класс реализует методы сервиса дерева Wiki модуля УП.
/// </summary>
internal sealed class WikiTreeService : IWikiTreeService
{
    private readonly ILogger<WikiTreeService>? _logger;
    private readonly IWikiTreeRepository _wikiTreeRepository;
    private readonly IUserRepository _userRepository;
    private const string FOLDER_ICON = "pi pi-folder";
    private const string FILE_ICON = "pi pi-file";

    /// <summary>
    /// Элементы дерева.
    /// </summary>
    private readonly List<WikiTreeItem> _treeItems = new();

    #region Публичные методы.

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="wikiTreeRepository">Репозиторий дерева.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    public WikiTreeService(ILogger<WikiTreeService>? logger,
        IWikiTreeRepository wikiTreeRepository,
        IUserRepository userRepository)
    {
        _logger = logger;
        _wikiTreeRepository = wikiTreeRepository;
        _userRepository = userRepository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<WikiTreeItem>> GetTreeAsync(long projectId)
    {
        try
        {
            // Получаем иерархию дерева папок.
            var folders = (await _wikiTreeRepository.GetFolderItemsAsync(projectId))?.AsList();

            if (folders is null || folders.Count == 0)
            {
                throw new InvalidOperationException("У проекта нет ни одной папки. " +
                                                    "Должны быть минимум системные папки." +
                                                    $"ProjectId: {projectId}.");
            }

            var foldersLinkedList = new LinkedList<WikiTreeItem>(folders);

            // Наполняем папки вложенными элементами (страницами или другими папками).
            var pages = (await _wikiTreeRepository.GetPageItemsAsync(folders.Select(x => x.FolderId),
                folders.Select(x => x.WikiTreeId)))?.AsList();

            // Рекурсивно обходим дерево и заполняем папки.
            await RecursiveBuildTreeAsync(foldersLinkedList.First!, folders, pages);
            
            return _treeItems;
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<WikiTreeItem>> GetTreeItemFolderAsync(long projectId, long folderId)
    {
        try
        {
            var childFolderIds = (await _wikiTreeRepository.GetChildFolderAsync(folderId))?.AsList();

            if (childFolderIds is null || childFolderIds.Count == 0)
            {
                return Enumerable.Empty<WikiTreeItem>();
            }

            childFolderIds.Add(folderId);

            var childFolders = (await _wikiTreeRepository.GetFoldersByFolderIdsAsync(childFolderIds))?.AsList();

            // Наполняем папки вложенными элементами (страницами или другими папками).
            var pages = (await _wikiTreeRepository.GetPageItemsAsync(childFolders.Select(x => x.FolderId).Distinct(),
                childFolders.Select(x => x.WikiTreeId).Distinct()))?.AsList();

            // Обходим дерево и заполняем папки.
            await BuildTreeAsync(childFolders.First(x => x.FolderId == folderId),
                childFolders.Where(x => x.FolderId != folderId).AsList(), pages);

            return _treeItems;
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<WikiTreeItem> GetTreeItemPageAsync(long pageId)
    {
        try
        {
            var result = await _wikiTreeRepository.GetTreeItemPageAsync(pageId);

            if (result is null)
            {
                throw new InvalidOperationException($"Ошибка получения страницы дерева. PageId: {pageId}");
            }

            return result!;
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateFolderNameAsync(string? folderName, long folderId)
    {
        try
        {
            await _wikiTreeRepository.UpdateFolderNameAsync(folderName, folderId);
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateFolderPageNameAsync(string? pageName, long pageId)
    {
        try
        {
            await _wikiTreeRepository.UpdateFolderPageNameAsync(pageName, pageId);
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateFolderPageDescriptionAsync(string? pageDescription, long pageId)
    {
        try
        {
            await _wikiTreeRepository.UpdateFolderPageDescriptionAsync(pageDescription, pageId);
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task CreateFolderAsync(long? parentId, string? folderName, string account, long treeId)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            await _wikiTreeRepository.CreateFolderAsync(parentId, folderName, userId, treeId);
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task CreatePageAsync(long? parentId, string? pageName, string account, long treeId)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            await _wikiTreeRepository.CreatePageAsync(parentId, pageName, userId, treeId);
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<RemoveFolderResponseOutput> RemoveFolderAsync(long folderId, bool isApprove)
    {
        try
        {
            if (!isApprove)
            {
                var isNeedUserAction = await _wikiTreeRepository.IfExistsFolderChildrenItemsAsync(folderId);

                if (isNeedUserAction)
                {
                    return new RemoveFolderResponseOutput
                    {
                        IsNeedUserAction = true,
                        ResponseText = "При удаление папки, будут удалены все дочерние элементы."
                    };
                }
            }

            await _wikiTreeRepository.RemoveFolderAsync(folderId);
            
            return new RemoveFolderResponseOutput
            {
                IsNeedUserAction = false
            };
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task RemovePageAsync(long pageId)
    {
        try
        {
              await _wikiTreeRepository.RemovePageAsync(pageId);
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод рекурсивно заполняет папки дерева вложенными элементами.
    /// </summary>
    /// <param name="folder">Папка текущего узла.</param>
    /// <param name="folders">Все папки проекта.</param>
    /// <param name="pages">Все страницы папок проекта.</param>
    private async Task RecursiveBuildTreeAsync(LinkedListNode<WikiTreeItem>? folder, List<WikiTreeItem> folders,
        List<WikiTreeItem>? pages)
    {
        // Если папка не пуста, то смотрим ее и ее детей.
        if (folder is not null)
        {
            // Родительская папка.
            folder.Value.Children ??= new List<WikiTreeItem>();
            folder.Value.Icon = FOLDER_ICON;

            // Работаем с дочерними папки в рамках родительской папки.
            var childFolders = folders.Where(x => x.ParentId == folder.Value.FolderId && !x.IsPage)?.AsList();

            // Если у текущей папки есть дочерние папки.
            if (childFolders is not null && childFolders.Count > 0)
            {
                // Перебираем дочерние папки родителя.
                foreach (var cf in childFolders)
                {
                    cf.Children ??= new List<WikiTreeItem>();
                    cf.Icon = FOLDER_ICON;

                    // Есть ли у дочерней папки тоже дети.
                    var childChildFolders = folders.Where(x => x.ParentId == cf.FolderId && !x.IsPage)?.AsList();

                    if (childChildFolders is not null && childChildFolders.Count > 0)
                    {
                        foreach (var c in cf.Children)
                        {
                            foreach (var c1 in childChildFolders)
                            {
                                if (c.FolderId != c1.FolderId && !c.IsPage)
                                {
                                    c.Children ??= new List<WikiTreeItem>();
                                    c.Children.Add(c1);
                                }
                            }
                        }
                    }
                    
                    if (pages is not null && pages.Count > 0)
                    {
                        // Если страницы принадлежат текущей папке, то добавляем их в нее.
                        if (pages.All(x => x.FolderId == folder.Value.FolderId))
                        {
                            var folderPages = pages.Select(x => new WikiTreeItem
                            {
                                Name = x.Name,
                                WikiTreeId = x.WikiTreeId,
                                PageId = x.PageId,
                                Icon = FILE_ICON,
                                FolderId = x.FolderId,
                                ProjectId = x.ProjectId,
                                CreatedBy = x.CreatedBy,
                                CreatedAt = x.CreatedAt,
                                ParentId = x.ParentId
                            });
                            
                            // Добавляем страницы текущей папки.
                            foreach (var page in folderPages)
                            {
                                if (!folder.Value.Children.Select(x => x.PageId).Contains(page.PageId))
                                {
                                    folder.Value.Children.Add(page);
                                }
                            }
                        }
                    }
                }

                folder.Value.Children.AddRange(childFolders);

                if (_treeItems.Count == 0)
                {
                     _treeItems.Add(folder.Value);
                }
                
                // Добавляем текущую папку и ее детей в дерево.
                // Если ее нету среди дочерних в дереве.
                else if (_treeItems.Count > 0
                         && await CanAddFolderTreeAsync(new LinkedList<WikiTreeItem>(_treeItems).First!))
                {
                    _treeItems.Add(folder.Value);
                }
            }

            // У текущей папки нету дочерних папок - добавляем как отдельную папку.
            else
            {
                // Смотрим, есть ли у такой папки страницы.
                if (pages is not null && pages.Count > 0)
                {
                    var currentFolderPages = pages
                        .Where(x => x.FolderId == folder.Value.FolderId)
                        .Select(c => new WikiTreeItem
                        {
                            Name = c.Name,
                            WikiTreeId = c.WikiTreeId,
                            PageId = c.PageId,
                            Icon = FILE_ICON
                        })?.AsList();

                    // У текущей папки есть дочерние страницы, добавим их ей.
                    if (currentFolderPages is not null && currentFolderPages.Count > 0)
                    {
                        folder.Value.Children ??= new List<WikiTreeItem>();
                        folder.Value.Children.AddRange(currentFolderPages);
                        
                        _treeItems.Add(new WikiTreeItem
                        {
                            Name = folder.Value.Name,
                            WikiTreeId = folder.Value.WikiTreeId,
                            Icon = FOLDER_ICON,
                            Children = folder.Value.Children,
                            FolderId = folder.Value.FolderId,
                            ProjectId = folder.Value.ProjectId,
                            CreatedBy = folder.Value.CreatedBy,
                            CreatedAt = folder.Value.CreatedAt
                        });
                    }
                }
            }

            // Можно ли добавлять в дерево.
            var isCanAdd = false;

            foreach (var ti in _treeItems)
            {
                // Если есть на 1 уровне уже такая папка, то запрещаем добавление в результат.
                if (ti.FolderId == folder.Value.FolderId)
                {
                    isCanAdd = false;
                    continue;
                }

                isCanAdd = true;
                ti.Children ??= new List<WikiTreeItem>();

                // Если есть на 2 уровне уже такая папка, то запрещаем добавление в результат.
                foreach (var child in ti.Children)
                {
                    if (child.FolderId == folder.Value.FolderId)
                    {
                        isCanAdd = false;
                        continue;
                    }
                    
                    isCanAdd = true;
                }
            }

            // Папки еще нету на 1 уровне дерева, добавляем ее, если ее нету среди дочерних папок.
            // if (isCanAdd && !_treeItems
            //         .SelectMany(a => a.Children ?? new List<WikiTreeItem>())
            //         .Select(b => b.FolderId)
            //         .Contains(folder.Value.FolderId))
            // {
            //     _treeItems.Add(new WikiTreeItem
            //     {
            //         Name = folder.Value.Name,
            //         WikiTreeId = folder.Value.WikiTreeId,
            //         Icon = FOLDER_ICON,
            //         Children = folder.Value.Children,
            //         FolderId = folder.Value.FolderId,
            //         ProjectId = folder.Value.ProjectId,
            //         CreatedBy = folder.Value.CreatedBy,
            //         CreatedAt = folder.Value.CreatedAt
            //     });
            // }

            if (isCanAdd && await CanAddFolderTreeAsync(new LinkedList<WikiTreeItem>(_treeItems).First!))
            {
                _treeItems.Add(new WikiTreeItem
                {
                    Name = folder.Value.Name,
                    WikiTreeId = folder.Value.WikiTreeId,
                    Icon = FOLDER_ICON,
                    Children = folder.Value.Children,
                    FolderId = folder.Value.FolderId,
                    ProjectId = folder.Value.ProjectId,
                    CreatedBy = folder.Value.CreatedBy,
                    CreatedAt = folder.Value.CreatedAt
                });
            }

            // Добавляем на 1 уровень дерева страницы, которые без родителя.
            if (pages is not null && pages.Count > 0)
            {
                _treeItems.AddRange(pages.Where(b => b.FolderId is null).Select(c => new WikiTreeItem
                {
                    Name = c.Name,
                    WikiTreeId = c.WikiTreeId,
                    PageId = c.PageId,
                    Icon = FILE_ICON
                })); 
            }

            // Переходим к следующему узлу, если его нет, то прекратим рекурсивный обход дерева.
            await RecursiveBuildTreeAsync(folder.Next, folders, pages);
        }
    }

    /// <summary>
    /// Метод получает структуру папки.
    /// </summary>
    /// <param name="folder">Папка.</param>
    /// <param name="folders">Дочерние папки.</param>
    /// <param name="pages">Страницы.</param>
    private async Task BuildTreeAsync(WikiTreeItem folder,
        IReadOnlyCollection<WikiTreeItem> folders, IReadOnlyCollection<WikiTreeItem>? pages)
    {
        // Родительская папка.
        folder.Children ??= new List<WikiTreeItem>();
        folder.Icon = FOLDER_ICON;

        // Перебираем дочерние папки родителя.
        foreach (var cf in folders)
        {
            cf.Children ??= new List<WikiTreeItem>();
            cf.Icon = FOLDER_ICON;

            // Есть ли у дочерней папки тоже дети.
            var childFolders = folders.Where(x => x.ParentId == cf.FolderId && !x.IsPage)?.AsList();

            if (childFolders is not null && childFolders.Count > 0)
            {
                foreach (var chf in childFolders)
                {
                    chf.Icon = FOLDER_ICON;
                }

                // TODO: А если еще есть дети ниже? То рекурсивно обходить, пока есть дети.
                cf.Children.AddRange(childFolders);

                if (pages is not null && pages.Count > 0)
                {
                    // Заполняем страницы дочерней папки.
                    await BuildFolderPagesAsync(cf, pages);
                }
            }
        }

        folder.Children.AddRange(folders);

        if (pages is not null && pages.Count > 0)
        {
            // Заполняем страницы дочерней папки.
            await BuildFolderPagesAsync(folder, pages);
        }

        // Добавляем родительскую папку в результат.
        _treeItems.Add(folder);
    }

    /// <summary>
    /// Метод наполняет папку вложенными страницами.
    /// </summary>
    /// <param name="treeItem">Папка.</param>
    /// <param name="pages">Все страницы из всех папок в памяти.</param>
    private async Task BuildFolderPagesAsync(WikiTreeItem treeItem, IReadOnlyCollection<WikiTreeItem> pages)
    {
        // Узкое место, на всякий делаем проверку на такое.
        if (treeItem.Children is null)
        {
            throw new InvalidOperationException(
                "Дочерние элементы папки оказались NULL, но к этому моменту должна была пройти инициализацию." +
                $"FolderId: {treeItem.FolderId}.");
        }

        // Перебираем страницы, которыми будем наполнять папки.
        foreach (var p in pages.Where(x => x.FolderId == treeItem.FolderId && x.IsPage))
        {
            p.Icon = FILE_ICON;

            if (treeItem.Children.Select(x => x.PageId).Contains(p.PageId))
            {
                continue;
            }
            
            // Заполняем дочернюю папку ее страницами.
            treeItem.Children.Add(p);
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Метод рекурсивно обходит дочерние папки дерева, чтобы выяснить, есть ли текущая папка среди дочерних в дереве.
    /// Если да, то запрещаем добавление папки в результат во избежание дублей.
    /// </summary>
    /// <param name="folders">Список папок.</param>
    /// <returns>Признак проверки.</returns>
    private async Task<bool> CanAddFolderTreeAsync(LinkedListNode<WikiTreeItem> folders)
    {
        if (folders.Value.Children is not null && folders.Value.Children.Count > 0)
        {
            while (folders.Value.Children.Count > 0)
            {
                foreach (var folder in folders.Value.Children)
                {
                    if (folder.FolderId == folders.Value.FolderId)
                    {
                        return false;
                    }
                
                    return true;
                }
            }
        }

        if (folders.Next is not null)
        {
            await CanAddFolderTreeAsync(folders.Next);
        }
        
        return true;
    }

    #endregion
}