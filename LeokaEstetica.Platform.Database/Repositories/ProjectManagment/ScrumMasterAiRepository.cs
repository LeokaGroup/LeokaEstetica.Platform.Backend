using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;

namespace LeokaEstetica.Platform.Database.Repositories.ProjectManagment;

/// <summary>
/// Класс реализует методы репозитория Scrum Master AI.
/// </summary>
internal sealed class ScrumMasterAiRepository : BaseRepository, IScrumMasterAiRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="connectionProvider">Провайдер подключения к БД.</param>
    public ScrumMasterAiRepository(IConnectionProvider connectionProvider) : base(connectionProvider)
    {
    }

    #region Публичные методы.

    /// <inheritdoc/>
    public async Task<string?> GetLastNetworkVersionAsync()
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var query = "SELECT version_number " +
                    "FROM ai.scrum_master_ai_message_versions " +
                    "ORDER BY DESC version_id " +
                    "LIMIT 1";

        var result = await connection.QueryFirstOrDefaultAsync(query);

        return result;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}