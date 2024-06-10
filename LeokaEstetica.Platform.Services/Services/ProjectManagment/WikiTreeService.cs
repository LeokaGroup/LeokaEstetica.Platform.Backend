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
    public async Task<IEnumerable<WikiTreeFolderItem>> GetTreeAsync(long projectId)
    {
        try
        {
            var result = new List<WikiTreeFolderItem>();
            
            // Получаем иерархию дерева папок.
            var folders = (await _wikiTreeRepository.GetFolderItemsAsync(projectId))?.AsList();

            if (folders is null || folders.Count == 0)
            {
                return result;
            }
            
            // Наполняем папки вложенными элементами (страницами или другими папками).
            var pages = (await _wikiTreeRepository.GetPageItemsAsync(folders.Select(x => x.FolderId),
                folders.Select(x => x.WikiTreeId)))?.AsList();
                
            if (pages is null || pages.Count == 0)
            {
                return result;
            }

            // Перебираем папки.
            foreach (var f in folders)
            {
                // Если у ппки есть вложенная папка.
                if (f.ChildId.HasValue)
                {
                    var childFolder = folders.FirstOrDefault(x => x.FolderId == f.ChildId.Value);

                    if (childFolder is null)
                    {
                        throw new InvalidOperationException("Дочерняя папка в дереве не найдена, но был Id потомка. " +
                                                            $"ChildId: {f.ChildId.Value}.");
                    }
                    
                    // Перебираем страницы, которыми будем наполнять папки.
                    var childFolderPages = pages.Where(x => x.FolderId == f.ChildId.Value).AsList();
                    
                    // Страниц нет, но папку добавим - будет пустой.
                    if (childFolderPages.Count == 0)
                    {
                        childFolder.Icon = "pi pi-folder";
                        
                        // Добавляем папку в результат.
                        result.Add(childFolder);
                        
                        continue;
                    }
                    
                    childFolder.Children ??= new List<WikiTreePageItem>();
                    
                    childFolderPages.ForEach(x => x.Icon = "pi pi-file");
                    
                    // Заполняем дочернюю папку ее страницами.
                    childFolder.Children.AddRange(childFolderPages);
                    
                    childFolder.Icon = "pi pi-folder";
                    
                    // Добавляем папку с вложенными в нее страницами в результат.
                    result.Add(childFolder);
                }
                
                // Перебираем страницы, которыми будем наполнять папки.
                var folderPages = pages.Where(x => x.FolderId == f.FolderId).AsList();
                folderPages.ForEach(x => x.Icon = "pi pi-file");

                // Страниц нет, но папку добавим - будет пустой.
                if (folderPages.Count == 0)
                {
                    f.Icon = "pi pi-folder";
                
                    // Добавляем папку в результат.
                    result.Add(f);
                    
                    continue;
                }
                
                f.Children ??= new List<WikiTreePageItem>();
                
                folderPages.ForEach(x => x.Icon = "pi pi-file");

                // Заполняем папку ее страницами.
                f.Children.AddRange(folderPages);
                
                f.Icon = "pi pi-folder";
                
                // Добавляем папку с вложенными в нее страницами в результат.
                result.Add(f);
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    #endregion
}