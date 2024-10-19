using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Database.Abstractions.Communications;
using LeokaEstetica.Platform.Models.Dto.Communications.Output;

namespace LeokaEstetica.Platform.Database.Repositories.Communications;

/// <summary>
/// Класс репозитория объектов группы.
/// </summary>
internal sealed class AbstractGroupObjectsRepository : BaseRepository, IAbstractGroupObjectsRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="connectionProvider">Провайдер к БД.</param>
    public AbstractGroupObjectsRepository(IConnectionProvider connectionProvider)
        : base(connectionProvider)
    {
    }

    /// <inheritdoc />
    public async Task<IEnumerable<GroupObjectDialogOutput>> GetObjectDialogsAsync(IEnumerable<long> objectIds)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@abstractScopeIds", objectIds.AsList());

        var query = "SELECT DISTINCT (id.dialog_id), " +
                    "dmes.message_id, " +
                    "(SELECT RIGHT(dmes.message, 40) " +
                    "FROM communications.dialog_messages " +
                    "WHERE dmes.dialog_id = id.dialog_id " +
                    "ORDER BY dmes.message DESC) AS last_message, " +
                    "TO_CHAR(id.created_at, 'dd.MM.yyyy HH24:MI'), " +
                    "dmes.created_by, " +
                    "id.dialog_name AS label, " +
                    "id.abstract_scope_id AS object_id " +
                    "FROM communications.main_info_dialogs AS id " +
                    "INNER JOIN communications.dialog_members AS dm " +
                    "ON id.dialog_id = dm.dialog_id " +
                    "LEFT JOIN communications.dialog_messages AS dmes " +
                    "ON id.dialog_id = dmes.dialog_id " +
                    "WHERE id.abstract_scope_id = ANY (@abstractScopeIds) ";

        var result = await connection.QueryAsync<GroupObjectDialogOutput>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<GroupObjectDialogMessageOutput>> GetObjectDialogMessagesAsync(long dialogId,
        long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@dialogId", dialogId);
        parameters.Add("@userId", userId);
        
        var query = "SELECT DISTINCT (dmes.message_id), " +
                    "dmes.message AS label, " +
                    "dmes.dialog_id, " +
                    "TO_CHAR(dmes.created_at, 'dd.MM.yyyy HH24:MI') AS created_at, " +
                    "dmes.created_by, " +
                    "dmes.message_id," +
                    "(CASE " +
                    "WHEN dmes.created_by = @userId " +
                    "THEN TRUE " +
                    "ELSE FALSE END) AS is_my_message " +
                    "FROM communications.main_info_dialogs AS id " +
                    "INNER JOIN communications.dialog_members AS dm " +
                    "ON id.dialog_id = dm.dialog_id " +
                    "LEFT JOIN communications.dialog_messages AS dmes " +
                    "ON id.dialog_id = dmes.dialog_id " +
                    "WHERE dmes.dialog_id = @dialogId ";

        var result = await connection.QueryAsync<GroupObjectDialogMessageOutput>(query, parameters);

        return result;
    }
}