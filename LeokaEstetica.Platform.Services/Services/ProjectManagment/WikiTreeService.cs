using System.Runtime.CompilerServices;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;
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
    // Список папок, которые удалим из 1 уровня, так как они уже будут на 2 и ниже уровнях.
    // Во избежание дублей папок на 1 уровне.
    /// </summary>
    private readonly List<long> _removedFolderIds = new(0);

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
            await RecursiveBuildTreeAsync(foldersLinkedList.First!, folders, pages, false);

            // TODO: По хорошему убрать этот костыль и разобраться с дублями по FolderId во всем дереве,
            // TODO: но это жестко будет ибо не так просто)
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
            // Получаем иерархию дерева папок.
            var folders = (await _wikiTreeRepository.GetFolderItemsAsync(projectId))
                ?.Where(x => x.FolderId == folderId)
                .AsList();
            
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
            await RecursiveBuildTreeAsync(foldersLinkedList.First!, folders, pages, true);
            
            // Удаляем папки 1 уровня дерева, так как они уже вложены на 2 уровне и ниже.
            // Иначе будут дубли на 1 уровне дерева.
            _treeItems.RemoveAll(x => _removedFolderIds.Contains(x.FolderId));

            return _treeItems.DistinctBy(x => new { x.FolderId, x.PageId });
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

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод рекурсивно заполняет папки дерева вложенными элементами.
    /// </summary>
    /// <param name="folder">Папка текущего узла.</param>
    /// <param name="folders">Все папки проекта.</param>
    /// <param name="pages">Все страницы папок проекта.</param>
    /// <param name="isSingleIteration">Если нужна лишь 1 итерация, например, при получении структуры папки.</param>
    private async Task RecursiveBuildTreeAsync(LinkedListNode<WikiTreeItem> folder, List<WikiTreeItem> folders,
        List<WikiTreeItem>? pages, bool isSingleIteration)
    {
        if (folder.Next?.Value is not null || isSingleIteration)
        {
            var f = folder.Value;

            // Если у папки есть вложенные папки.
            if (f.ChildId.HasValue)
            {
                // Работаем с дочерними папки в рамках родительской папки.
                var childFolders = folders.Where(x => x.ParentId == f.FolderId && !x.IsPage)?.AsList();

                // Дочерние папки родительской.
                if (childFolders is not null && childFolders.Count > 0)
                {
                    // Перебираем дочерние папки родителя.
                    foreach (var cf in childFolders)
                    {
                        cf.Children ??= new List<WikiTreeItem>();
                        cf.Icon = "pi pi-folder";

                        var parentFolder = folders.FirstOrDefault(x => x.FolderId == cf.ParentId && !x.IsPage);

                        if (parentFolder is not null)
                        {
                            parentFolder.Children ??= new List<WikiTreeItem>();

                            if (!parentFolder.IsPage)
                            {
                                parentFolder.Icon = "pi pi-folder";

                                // Добавляем в родительскую папку ее дочернюю папку.
                                parentFolder.Children.Add(cf);
                            }
                        }

                        var childFolderPages = pages?.Where(x => x.FolderId == cf.FolderId)?.AsList();
                        
                        // Если есть страницы.
                        if (childFolderPages is not null && childFolderPages.Count > 0)
                        {
                            // Заполняем страницы дочерней папки.
                            await BuildFolderPagesAsync(cf, pages, isSingleIteration);
                        }
                    }
                }

                // Если нету у родителя дочерних папок, обрабатываем только родительскую папку.
                else
                {
                    var childFolderPages = pages?.Where(x => x.FolderId == f.FolderId)?.AsList();
                    
                    // Если есть страницы.
                    if (childFolderPages is not null && childFolderPages.Count > 0)
                    {
                        // Заполняем страницы родительской папки.
                        await BuildFolderPagesAsync(f, pages, isSingleIteration);
                    }
                }
            }
            
            var parentFolderPages = pages?.Where(x => x.FolderId == f.FolderId)?.AsList();
            
            // Если есть страницы у род.папки.
            if (parentFolderPages is not null && parentFolderPages.Count > 0)
            {
                await BuildFolderPagesAsync(f, pages, isSingleIteration);   
            }

            // Прошли 1 итерацию, больше не нужно.
            if (isSingleIteration)
            {
                return;
            }

            // Переходим к следующему узлу, если его нет, то прекратим рекурсивный обход дерева.
            await RecursiveBuildTreeAsync(folder.Next!, folders, pages, isSingleIteration);
        }
    }

    /// <summary>
    /// Метод наполняет папку вложенными страницами.
    /// </summary>
    /// <param name="treeItem">Папка.</param>
    /// <param name="pages">Все страницы из всех папок в памяти.</param>
    private async Task BuildFolderPagesAsync(WikiTreeItem treeItem, List<WikiTreeItem> pages, bool isSingleIteration)
    {
        // Перебираем страницы, которыми будем наполнять папки.
        var childFolderPages = pages.Where(x => x.FolderId == treeItem.FolderId && x.IsPage)?.AsList();
        
        treeItem.Icon = "pi pi-folder";
        treeItem.Children ??= new List<WikiTreeItem>();
                    
        // Страниц нет, но папку добавим - будет пустой.
        if (childFolderPages is null || childFolderPages.Count == 0)
        {
            // Добавляем папку в результат.
            _treeItems.Add(treeItem);

            return;
        }

        foreach (var p in childFolderPages)
        {
            p.Icon = !p.IsPage ? "pi pi-folder" : "pi pi-file";
        }
        
        // Заполняем дочернюю папку ее страницами.
        treeItem.Children.AddRange(childFolderPages);

        // Добавляем папку с вложенными в нее страницами в результат.
        _treeItems.Add(treeItem);

        if (!isSingleIteration)
        {
            // Заполняем дочернюю папку ее страницами.
            treeItem.Children.AddRange(childFolderPages);
        
            // Добавляем папку с вложенными в нее страницами в результат.
            _treeItems.Add(treeItem);
        }
        
        else
        {
            _treeItems.AddRange(childFolderPages.Where(x => x.IsPage));
        }

        await Task.CompletedTask;
    }

    #endregion
}