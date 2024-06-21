using System.Runtime.CompilerServices;
using Dapper;
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
    public WikiTreeService(ILogger<WikiTreeService>? logger,
     IWikiTreeRepository wikiTreeRepository)
    {
        _logger = logger;
        _wikiTreeRepository = wikiTreeRepository;
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

            // Удаляем папки 1 уровня дерева, так как они уже вложены на 2 уровне и ниже.
            // Иначе будут дубли на 1 уровне дерева.
            _treeItems.RemoveAll(x => _removedFolderIds.Contains(x.FolderId));

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
            var result = await _wikiTreeRepository.GetFolderStructureAsync(projectId, folderId);

            return result!;
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

    #endregion

    #region Приватные методы.

    private async Task RecursiveBuildTreeAsync(LinkedListNode<WikiTreeItem> folder, List<WikiTreeItem> folders, List<WikiTreeItem>? pages)
    {
        if (folder.Next?.Value is not null)
        {
            var f = folder.Value;

            // Если у папки есть вложенные папки.
            if (f.ChildId.HasValue)
            {
                // Работаем с дочерними папки в рамках родительской папки.
                var childFolders = folders.Where(x => x.FolderId == f.ChildId.Value && !x.IsPage)?.AsList();

                // Дочерние папки родительской.
                if (childFolders is not null && childFolders.Count > 0)
                {
                    _removedFolderIds.AddRange(childFolders.Select(x => x.FolderId));

                    // Перебираем дочерние папки родителя.
                    foreach (var cf in childFolders)
                    {
                        cf.Children ??= new List<WikiTreeItem>();

                        var parentFolder = folders.FirstOrDefault(x => x.FolderId == cf.ParentId && !x.IsPage);

                        if (parentFolder is not null)
                        {
                            parentFolder.Children ??= new List<WikiTreeItem>();

                            if (!parentFolder.IsPage)
                            {
                                parentFolder.Icon = "pi pi-folder";

                                parentFolder.Children.Add(cf);
                                _treeItems.Add(parentFolder);
                            }
                        }

                        // Если есть страницы.
                        if (pages is not null && pages.Count > 0)
                        {
                            await BuildFolderPagesAsync(cf, pages, _treeItems);
                        }
                    }
                }

                // Если нету у родителя дочерних папок, обрабатываем только родительскую папку.
                else
                {
                    // Если есть страницы.
                    if (pages is not null && pages.Count > 0)
                    {
                        await BuildFolderPagesAsync(f, pages, _treeItems);
                    }
                }
            }

            // Переходим к следующему узлу, если его нет, то прекратим рекурсивный обход дерева.
            await RecursiveBuildTreeAsync(folder.Next, folders, pages);
        }
    }

    /// <summary>
    /// Метод наполняет папку вложенными страницами.
    /// </summary>
    /// <param name="treeItem">Папка.</param>
    /// <param name="pages">Все страницы из всех папок в памяти.</param>
    /// <param name="result">Результат, который наполняется.</param>
    private async Task BuildFolderPagesAsync(WikiTreeItem treeItem, List<WikiTreeItem> pages,
        List<WikiTreeItem> result)
    {
        // Перебираем страницы, которыми будем наполнять папки.
        var childFolderPages = pages.Where(x => x.FolderId == (treeItem.ChildId ?? 0) && x.IsPage)?.AsList();
        
        treeItem.Icon = "pi pi-folder";
        treeItem.Children ??= new List<WikiTreeItem>();
                    
        // Страниц нет, но папку добавим - будет пустой.
        if (childFolderPages is null || childFolderPages.Count == 0)
        {
            // Добавляем папку в результат.
            result.Add(treeItem);

            return;
        }

        foreach (var p in childFolderPages)
        {
            p.Icon = "pi pi-file";
        }

        // Заполняем дочернюю папку ее страницами.
        treeItem.Children.AddRange(childFolderPages);

        // Добавляем папку с вложенными в нее страницами в результат.
        result.Add(treeItem);

        await Task.CompletedTask;
    }

    #endregion
}