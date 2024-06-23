using System.Runtime.CompilerServices;
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

    /// <summary>
    /// Элементы дерева.
    /// </summary>
    private readonly List<WikiTreeItem> _treeItems = new();

    /// <summary>
    /// Список папок, которые удаляем из дерева на 1 уровне, так как они есть на 2 уровне и ниже.
    /// Во избежание дублей на 1 уровне дерева.
    /// </summary>
    private readonly List<long> _removedFolderIds = new();

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
            var pages = (await _wikiTreeRepository.GetPageItemsAsync(folders.Select(x => x.FolderId).Distinct(),
                folders.Select(x => x.WikiTreeId).Distinct()))?.AsList();

            // Рекурсивно обходим дерево и заполняем папки.
            await RecursiveBuildTreeAsync(foldersLinkedList.First!, folders, pages);

            // TODO: Относительный костыль, иначе были дубли на 1 уровне дерева из папок,
            // TODO: которые есть в дочерних у какой то папки.
            _treeItems.RemoveAll(x => _removedFolderIds.Contains(x.FolderId ?? 0));

            // TODO: Дубли на 1 уровне дерева.
            return _treeItems.DistinctBy(x => x.FolderId);
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

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод рекурсивно заполняет папки дерева вложенными элементами.
    /// </summary>
    /// <param name="folder">Папка текущего узла.</param>
    /// <param name="folders">Все папки проекта.</param>
    /// <param name="pages">Все страницы папок проекта.</param>
    private async Task RecursiveBuildTreeAsync(LinkedListNode<WikiTreeItem> folder,
        List<WikiTreeItem> folders, List<WikiTreeItem>? pages)
    {
        if (folder.Next?.Value is not null)
        {
            // Родительская папка.
            folder.Value.Children ??= new List<WikiTreeItem>();
            folder.Value.Icon = "pi pi-folder";

            // Работаем с дочерними папки в рамках родительской папки.
            var childFolders = folders.Where(x => x.ParentId == folder.Value.FolderId && !x.IsPage)?.AsList();

            if (childFolders is not null && childFolders.Count > 0)
            {
                // Перебираем дочерние папки родителя.
                foreach (var cf in childFolders)
                {
                    cf.Children ??= new List<WikiTreeItem>();
                    cf.Icon = "pi pi-folder";

                    // Есть ли у дочерней папки тоже дети.
                    var childChildFolders = folders.Where(x => x.ParentId == cf.FolderId && !x.IsPage)?.AsList();

                    if (childChildFolders is not null && childChildFolders.Count > 0)
                    {
                        foreach (var c in cf.Children)
                        {
                            foreach (var c1 in childChildFolders)
                            {
                                if (c.FolderId != c1.FolderId)
                                {
                                    c.Children ??= new List<WikiTreeItem>();
                                    c.Children.Add(c1);
                                }
                            }

                            if (pages is not null && pages.Count > 0)
                            {
                                // Заполняем страницы дочерней папки.
                                await BuildFolderPagesAsync(cf, pages);
                            }
                        }
                    }
                }

                _removedFolderIds.AddRange(childFolders.Select(x => x.FolderId!.Value));

                folder.Value.Children.AddRange(childFolders);

                if (pages is not null && pages.Count > 0)
                {
                    // Заполняем страницы дочерней папки.
                    await BuildFolderPagesAsync(folder.Value, pages);
                }

                // Добавляем родительскую папку в результат.
                _treeItems.Add(folder.Value);
            }

            // Нет детей, значит добавляем на 1 уровень дерева.
            else
            {
                _treeItems.Add(folder.Value);
            }

            // Переходим к следующему узлу, если его нет, то прекратим рекурсивный обход дерева.
            await RecursiveBuildTreeAsync(folder.Next!, folders, pages);
        }

        // Добавляем на 1 уровень дерева страницы, которые без родителя.
        _treeItems.AddRange(pages!.Where(b => b.FolderId is null).Select(c => new WikiTreeItem
        {
            Name = c.Name,
            WikiTreeId = c.WikiTreeId,
            PageId = c.PageId,
            Icon = "pi pi-file"
        }));
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
        folder.Icon = "pi pi-folder";

        // Перебираем дочерние папки родителя.
        foreach (var cf in folders)
        {
            cf.Children ??= new List<WikiTreeItem>();
            cf.Icon = "pi pi-folder";

            // Есть ли у дочерней папки тоже дети.
            var childFolders = folders.Where(x => x.ParentId == cf.FolderId && !x.IsPage)?.AsList();

            if (childFolders is not null && childFolders.Count > 0)
            {
                foreach (var chf in childFolders)
                {
                    chf.Icon = "pi pi-folder";
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
        // Перебираем страницы, которыми будем наполнять папки.
        var folderPages = pages.Where(x => x.FolderId == treeItem.FolderId && x.IsPage)?.AsList();

        // Страниц нет, но папку добавим - будет пустой.
        if (folderPages is null || folderPages.Count == 0)
        {
            return;
        }

        foreach (var p in folderPages)
        {
            p.Icon = "pi pi-file";
        }

        // Узкое место, на всякий делаем проверку на такое.
        if (treeItem.Children is null)
        {
            throw new InvalidOperationException(
                "Дочерние элементы папки оказались NULL, но к этому моменту должна была пройти инициализация." +
                $"FolderId: {treeItem.FolderId}.");
        }

        // Заполняем дочернюю папку ее страницами.
        treeItem.Children.AddRange(folderPages);

        await Task.CompletedTask;
    }

    #endregion
}