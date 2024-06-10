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

    #endregion

    #region Приватные методы.

    

    #endregion
}