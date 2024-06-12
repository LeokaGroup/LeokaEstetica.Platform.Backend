using System.Data;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;

namespace LeokaEstetica.Platform.Database.Repositories.ProjectManagment;

/// <summary>
/// Класс реализует методы репозитория дерева Wiki модуля УП.
/// </summary>
internal sealed class WikiTreeRepository : BaseRepository, IWikiTreeRepository
{
    /// <summary>
    /// Структура папки (со вложенными папками и страницами - дочерними).
    /// </summary>
    private readonly (IEnumerable<WikiTreeFolderItem>? Folders, IEnumerable<WikiTreePageItem>? Pages)
        _folderStructures = (new List<WikiTreeFolderItem>(), new List<WikiTreePageItem>());

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="connectionProvider">Провайдер БД.</param>
    public WikiTreeRepository(IConnectionProvider connectionProvider)
        : base(connectionProvider)
    {
    }
    
    #region Публичные методы.

    /// <inheritdoc />
    public async Task<IEnumerable<WikiTreeFolderItem>?> GetFolderItemsAsync(long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);

        var query = "SELECT tf.folder_id," +
                    "tf.wiki_tree_id," +
                    "tf.folder_name," +
                    "tf.parent_id," +
                    "tf.child_id," +
                    "tf.created_by," +
                    "tf.created_at," +
                    "t.project_id " +
                    "FROM project_management.wiki_tree_folders AS tf " +
                    "INNER JOIN project_management.wiki_tree AS t " +
                    "ON tf.wiki_tree_id = t.wiki_tree_id " +
                    "WHERE t.project_id = @projectId " +
                    "ORDER BY tf.folder_id";

        var result = await connection.QueryAsync<WikiTreeFolderItem>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<WikiTreePageItem>?> GetPageItemsAsync(IEnumerable<long> folderIds,
        IEnumerable<long> treeIds)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@folderIds", folderIds.AsList());
        parameters.Add("@treeIds", treeIds.AsList());

        var query = "SELECT p.page_id," +
                    "p.folder_id," +
                    "p.page_name," +
                    "p.page_description," +
                    "p.wiki_tree_id," +
                    "p.created_by," +
                    "p.created_at " +
                    "FROM project_management.wiki_tree_folders AS tf " +
                    "LEFT JOIN project_management.wiki_tree_pages AS p " +
                    "ON tf.folder_id = p.folder_id " +
                    "WHERE p.folder_id = ANY(@folderIds) " +
                    "AND p.wiki_tree_id = ANY(@treeIds)";

        var result = await connection.QueryAsync<WikiTreePageItem>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task CreateProjectWikiAsync(long projectId, long userId, string? projectName)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        using var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            var lastWikiTreeQuery = "SELECT wiki_tree_id " +
                                    "FROM project_management.wiki_tree " +
                                    "ORDER BY wiki_id DESC " +
                                    "LIMIT 1";
            
            // Получаем последний Id дерева.
            var lastWikiTreeId = await connection.ExecuteScalarAsync<long>(lastWikiTreeQuery);
            var wikiTreeId = ++lastWikiTreeId;
            
            var insertWikiParameters = new DynamicParameters();
            insertWikiParameters.Add("@projectId", projectId);
            insertWikiParameters.Add("@wikiTreeId", wikiTreeId);
            
            // Добавляем Wiki проекта.
            var insertWikiQuery = "INSERT INTO project_management.wiki_tree (wiki_tree_id, project_id) " +
                                  "VALUES (@wikiTreeId, @projectId)";
            
            await connection.ExecuteAsync(insertWikiQuery, insertWikiParameters);
            
            // Заводим ознакомительную папку для wiki проекта.
            var insertWikiFolderParameters = new DynamicParameters();
            insertWikiFolderParameters.Add("@wikiTreeId", wikiTreeId);
            insertWikiFolderParameters.Add("@folderName", "Перед началом работы");
            insertWikiFolderParameters.Add("@createdBy", userId);

            var insertWikiFolderQuery = "INSERT INTO project_management.wiki_tree_folders (" +
                                        "wiki_tree_id, folder_name, created_by) " +
                                        "VALUES (@wikiTreeId, @folderName, @createdBy) " +
                                        "RETURNING folder_id";

            var folderId = await connection.ExecuteScalarAsync<long>(insertWikiFolderQuery,
                insertWikiFolderParameters);

            // Заводим ознакомительную страницу для ознакомительной папки.
            var insertWikiFolderPageParameters = new DynamicParameters();
            insertWikiFolderPageParameters.Add("@wikiTreeId", wikiTreeId);
            insertWikiFolderPageParameters.Add("@folderId", folderId);
            insertWikiFolderPageParameters.Add("@pageName", "Начало работы");
            insertWikiFolderPageParameters.Add("@pageDescription",
                $"Это начальная страница проекта “{projectName}”. " +
                "Добавьте содержимое ознакомительной странице Вашего проекта.");
            insertWikiFolderPageParameters.Add("@wikiTreeId", wikiTreeId);
            insertWikiFolderPageParameters.Add("@createdBy", userId);

            var insertWikiFolderPageQuery = "INSERT INTO project_management.wiki_tree_pages (" +
                                            "folder_id, page_name, page_description, wiki_tree_id, created_by) " +
                                            "VALUES (@folderId, @pageName, @pageDescription, @wikiTreeId," +
                                            " @createdBy) " +
                                            "RETURNING page_id";

            var pageId = await connection.ExecuteScalarAsync(insertWikiFolderPageQuery,
                insertWikiFolderPageParameters);
            
            // Назначаем добавленную страницу папке.
            var updateChildFolderPageParameters = new DynamicParameters();
            updateChildFolderPageParameters.Add("@pageId", pageId);
            updateChildFolderPageParameters.Add("@folderId", folderId);

            var updateChildFolderPageQuery = "UPDATE project_management.wiki_tree_folders " +
                                             "SET child_id = @pageId " +
                                             "WHERE folder_id = @folderId";
                                             
            await connection.ExecuteAsync(updateChildFolderPageQuery, updateChildFolderPageParameters);

            transaction.Commit();
        }

        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<WikiTreeFolderItem>? Folders, IEnumerable<WikiTreePageItem>? Pages)>
        GetFolderStructureAsync(long projectId, long folderId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@folderId", folderId);

        var query = "SELECT tf.folder_id," +
                    "tf.wiki_tree_id," +
                    "tf.folder_name," +
                    "tf.parent_id," +
                    "tf.child_id," +
                    "tf.created_by," +
                    "tf.created_at " +
                    "FROM project_management.wiki_tree AS wt " +
                    "INNER JOIN project_management.wiki_tree_folders AS tf " +
                    "ON wt.wiki_tree_id = tf.wiki_tree_id ";
        
        // Получаем структуру папки. Эта папка будет являться верхним уровнем, так как ее выбрали.
        var folder = await connection.QueryFirstOrDefaultAsync<WikiTreeFolderItem>(query, parameters);

        if (folder is null)
        {
            throw new InvalidOperationException("Не удалось найти папку. " +
                                                $"FolderId: {folderId}. " +
                                                $"ProjectId: {projectId}.");
        }
        
        // Детей нет, вернем просто эту папку.
        if (!folder.ChildId.HasValue)
        {
            _folderStructures.Folders.AsList().Add(folder);
            
            return _folderStructures;
        }
        
        // Временный список - нужен для рекурсии.
        var tempChildFolders = new List<WikiTreeFolderItem>(1) { folder };

        await RecursiveBuildFoldersAsync(projectId, connection, tempChildFolders);

        return _folderStructures;
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод ресурсивно наполняет структуру папки (наполняя дочерними папками).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="connection">Подключение к БД.</param>
    /// <param name="tempChildFolders">Временный список (нужен только внутри рекурсии).</param>
    private async Task RecursiveBuildFoldersAsync(long projectId, IDbConnection connection,
        List<WikiTreeFolderItem> tempChildFolders)
    {
        // Рекурсивно наполняем детей родительской папки, если они есть.
        while (tempChildFolders.Count > 0)
        {
            // Дети есть - получаем все вложенные папки, если они есть.
            var childFoldersParameters = new DynamicParameters();
            childFoldersParameters.Add("@parentIdIds", tempChildFolders.Select(x => x.ParentId).AsList());
            childFoldersParameters.Add("@projectId", projectId);

            var childFoldersQuery = "SELECT tf.folder_id," +
                                    "tf.wiki_tree_id," +
                                    "tf.folder_name," +
                                    "tf.parent_id," +
                                    "tf.child_id," +
                                    "tf.created_by," +
                                    "tf.created_at " +
                                    "FROM project_management.wiki_tree AS wt " +
                                    "INNER JOIN project_management.wiki_tree_folders AS tf " +
                                    "ON wt.wiki_tree_id = tf.wiki_tree_id " +
                                    "WHERE tf.parent_id = ANY(@parentIdIds) " +
                                    "AND wt.project_id = @projectId";

            var childFolders = (await connection.QueryAsync<WikiTreeFolderItem>(childFoldersQuery,
                childFoldersParameters))?.AsList();

            if (childFolders is not null && childFolders.Count > 0)
            {
                _folderStructures.Folders.AsList().AddRange(childFolders);
                
                // Во избежание утечек памяти.
                tempChildFolders.Clear();
                tempChildFolders.AddRange(childFolders);
                
                // Ресайзим размер списка до фактического.
                tempChildFolders.TrimExcess();
            }

            else
            {
                // Во избежание утечек памяти.
                tempChildFolders.Clear();
                
                // Ресайзим размер списка до фактического, к нулю.
                tempChildFolders.TrimExcess();
            }
        }
    }

    #endregion
}