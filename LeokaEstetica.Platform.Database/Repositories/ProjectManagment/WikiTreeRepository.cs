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
    private readonly IEnumerable<WikiTreeItem>? _folders = new List<WikiTreeItem>();

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
    public async Task<IEnumerable<WikiTreeItem>?> GetFolderItemsAsync(long projectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);

        var query = "SELECT tf.folder_id," +
                    "tf.wiki_tree_id," +
                    "tf.folder_name AS Name," +
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

        var result = await connection.QueryAsync<WikiTreeItem>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<WikiTreeItem>?> GetPageItemsAsync(IEnumerable<long> folderIds,
        IEnumerable<long> treeIds)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@folderIds", folderIds.AsList());
        parameters.Add("@treeIds", treeIds.AsList());

        var query = "SELECT p.page_id," +
                    "p.folder_id," +
                    "p.page_name AS Name," +
                    "p.page_description," +
                    "p.wiki_tree_id," +
                    "p.created_by," +
                    "p.created_at " +
                    "FROM project_management.wiki_tree_folders AS tf " +
                    "LEFT JOIN project_management.wiki_tree_pages AS p " +
                    "ON tf.folder_id = p.folder_id " +
                    "WHERE p.folder_id = ANY(@folderIds) " +
                    "AND p.wiki_tree_id = ANY(@treeIds)";

        var result = await connection.QueryAsync<WikiTreeItem>(query, parameters);

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
    public async Task<IEnumerable<WikiTreeItem>?> GetFolderStructureAsync(long projectId, long folderId)
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
        var folder = await connection.QueryFirstOrDefaultAsync<WikiTreeItem>(query, parameters);

        if (folder is null)
        {
            throw new InvalidOperationException("Не удалось найти папку. " +
                                                $"FolderId: {folderId}. " +
                                                $"ProjectId: {projectId}.");
        }
        
        // Детей нет, но проверим, есть ли дочерние страницы у папки.
        // Если есть - заполним папку страницами, иначе просто вернем папку.
        if (!folder.ChildId.HasValue)
        {
            // Заполняем папки вложенными страницами.
            await GetFolderPagesAsync(new List<WikiTreeItem> { folder }, connection);

            _folders.AsList().Add(folder);
            
            return _folders;
        }
        
        // Временный список - нужен для рекурсии.
        // Чтобы начать с выбранной папки.
        var tempChildFolders = new List<WikiTreeItem>(1) { folder };

        await RecursiveBuildChildFolderStructureAsync(projectId, connection, tempChildFolders);

        return _folders;
    }

    /// <inheritdoc />
    public async Task<WikiTreeItem?> GetTreeItemPageAsync(long pageId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@pageId", pageId);

        var query = "SELECT page_id," +
                    "folder_id," +
                    "page_name," +
                    "page_description," +
                    "wiki_tree_id," +
                    "created_by," +
                    "created_at " +
                    "FROM project_management.wiki_tree_pages " +
                    "WHERE page_id = @pageId";

        var result = await connection.QueryFirstOrDefaultAsync<WikiTreeItem>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task UpdateFolderNameAsync(string? folderName, long folderId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@folderName", folderName);
        parameters.Add("@folderId", folderId);

        var query = "UPDATE project_management.wiki_tree_folders " +
                    "SET folder_name = @folderName " +
                    "WHERE folder_id = @folderId";

        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc />
    public async Task UpdateFolderPageNameAsync(string? pageName, long pageId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@pageName", pageName);
        parameters.Add("@pageId", pageId);

        var query = "UPDATE project_management.wiki_tree_pages " +
                    "SET page_name = @pageName " +
                    "WHERE page_id = @pageId";

        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc />
    public async Task UpdateFolderPageDescriptionAsync(string? pageDescription, long pageId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@pageDescription", pageDescription);
        parameters.Add("@pageId", pageId);

        var query = "UPDATE project_management.wiki_tree_pages " +
                    "SET page_description = @pageDescription " +
                    "WHERE page_id = @pageId";

        await connection.ExecuteAsync(query, parameters);
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод рекурсивно наполняет структуру папки (наполняя дочерними папками и страницами).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="connection">Подключение к БД.</param>
    /// <param name="tempChildFolders">Временный список (нужен только внутри рекурсии).</param>
    private async Task RecursiveBuildChildFolderStructureAsync(long projectId, IDbConnection connection,
        List<WikiTreeItem> tempChildFolders)
    {
        if (tempChildFolders.Count == 0)
        {
            return;
        }

        var childIds = tempChildFolders
            .Where(x => x.ChildId is not null)
            .Select(x => x.ChildId)
            .AsList();
        
        // Рекурсивно наполняем детей родительской папки, если они есть.
        // Дети есть - получаем все вложенные папки, если они есть.
        var childFoldersParameters = new DynamicParameters();
        childFoldersParameters.Add("@childIds", childIds);
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
                                "WHERE tf.folder_id = ANY(@childIds) " +
                                "AND wt.project_id = @projectId";

        var childFolders = (await connection.QueryAsync<WikiTreeItem>(childFoldersQuery,
            childFoldersParameters))?.AsList();

        if (childFolders is not null && childFolders.Count > 0)
        {
            // Заполняем папки вложенными страницами.
            await GetFolderPagesAsync(childFolders, connection);
            
            childFolders.ForEach(x => x.Icon = "pi pi-folder");
            
            _folders.AsList().AddRange(childFolders);

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

        await RecursiveBuildChildFolderStructureAsync(projectId, connection, tempChildFolders);
    }

    /// <summary>
    /// Метод заполняет папки дочерними страницами.
    /// </summary>
    /// <param name="childFolders">Список папок.</param>
    /// <param name="connection">Подключение к БД.</param>
    private async Task GetFolderPagesAsync(List<WikiTreeItem> childFolders, IDbConnection connection)
    {
        // Заполняем папки дочерними страницами.
        var childFolderPagesParameters = new DynamicParameters();
        childFolderPagesParameters.Add("@folderIds", childFolders.Select(x => x.FolderId).AsList());
        childFolderPagesParameters.Add("@wikiTreeIds", childFolders.Select(x => x.WikiTreeId).Distinct().AsList());

        var childFolderPagesQuery = "SELECT p.page_id," +
                                    "p.folder_id," +
                                    "p.page_name," +
                                    "p.page_description," +
                                    "p.wiki_tree_id," +
                                    "p.created_by," +
                                    "p.created_at " +
                                    "FROM project_management.wiki_tree_pages AS p " +
                                    "INNER JOIN project_management.wiki_tree_folders AS tf " +
                                    "ON p.folder_id = tf.folder_id " +
                                    "WHERE p.folder_id = ANY(@folderIds) " +
                                    "AND p.wiki_tree_id = ANY(@wikiTreeIds)";

        var pages = (await connection.QueryAsync<WikiTreeItem>(childFolderPagesQuery,
            childFolderPagesParameters))?.AsList();

        if (pages is not null && pages.Count > 0)
        {
            // Заполняем папки вложенными страницами.
            foreach (var f in childFolders)
            {
                // Страницы папки.
                var folderPages = pages.Where(x => x.FolderId == f.FolderId
                                                   && x.WikiTreeId == f.WikiTreeId)
                    .AsList();

                if (folderPages.Count > 0)
                {
                    if (f.Children is null)
                    {
                        f.Children = new List<WikiTreeItem>();
                    }
                        
                    folderPages.ForEach(x => x.Icon = "pi pi-file");
                        
                    // Наполняем папку вложенными в нее страницами.
                    f.Children.AddRange(folderPages);
                }

                f.Icon = "pi pi-folder";
            }
        }
    }

    #endregion
}