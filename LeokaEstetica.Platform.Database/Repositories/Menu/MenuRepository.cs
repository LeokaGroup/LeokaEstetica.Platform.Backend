using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Database.Abstractions.Menu;

namespace LeokaEstetica.Platform.Database.Repositories.Menu;

/// <summary>
/// Класс реализует методы репозитория меню.
/// </summary>
internal sealed class MenuRepository : BaseRepository, IMenuRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="connectionProvider">Провайдер к БД.</param>
    public MenuRepository(IConnectionProvider connectionProvider) : base(connectionProvider)
    {
    }

    #region Публичные методы.

    /// <inheritdoc />
    public async Task<string?> GetTopMenuItemsAsync()
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var query = "SELECT items " +
                    "FROM dbo.top_menu";

        var result = await connection.QueryFirstOrDefaultAsync<string>(query);

        return result;
    }

    /// <inheritdoc />
    public async Task<string?> GetLeftMenuItemsAsync()
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var query = "SELECT items " +
                    "FROM dbo.left_menu";

        var result = await connection.QueryFirstOrDefaultAsync<string>(query);

        return result;
    }

    /// <inheritdoc />
    public async Task<string?> GetProjectManagementLineMenuAsync()
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var query = "SELECT items " +
                    "FROM project_management.project_management_line_menu";

        var result = await connection.QueryFirstOrDefaultAsync<string>(query);

        return result;
    }

    /// <inheritdoc />
    public async Task<string?> GetLandingMenuItemsAsync()
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var query = "SELECT items " +
                    "FROM dbo.landing_menu";

        var result = await connection.QueryFirstOrDefaultAsync<string>(query);

        return result;
    }

    /// <inheritdoc />
    public async Task<string?> GetGroupObjectMenuItemsAsync()
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var query = "SELECT items " +
                    "FROM communications.group_object_actions_menu";

        var result = await connection.QueryFirstOrDefaultAsync<string>(query);

        return result;
    }

    /// <inheritdoc />
    public async Task<string?> GetDialogGroupMenuItemsAsync()
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var query = "SELECT items " +
                    "FROM communications.dialog_group_menu";

        var result = await connection.QueryFirstOrDefaultAsync<string>(query);

        return result;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}