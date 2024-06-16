﻿using System.Runtime.CompilerServices;
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

    /// <inheritdoc />
    public async Task<IEnumerable<WikiTreeFolderItem>> GetTreeItemFolderAsync(long projectId, long folderId)
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
    public async Task<WikiTreePageItem> GetTreeItemPageAsync(long pageId)
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

    #endregion
}