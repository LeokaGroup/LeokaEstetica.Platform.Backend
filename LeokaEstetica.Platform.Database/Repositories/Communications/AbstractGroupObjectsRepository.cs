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
    public async Task<IEnumerable<GroupObjectDialogMessageOutput>> GetObjectDialogsAsync(long abstractScopeId,
        long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@abstractScopeId", abstractScopeId);
        parameters.Add("@userId", userId);

        var query = "SELECT dmes.message_id, " +
                    "dmes.message, " +
                    "id.dialog_id, " +
                    "TO_CHAR(dmes.created_at, 'dd.MM.yyyy HH24:MI'), " +
                    "dmes.created_by," +
                    "id.dialog_name AS label " +
                    "FROM communications.main_info_dialogs AS id " +
                    "INNER JOIN communications.dialog_members AS dm " +
                    "ON id.dialog_id = dm.dialog_id " +
                    "LEFT JOIN communications.dialog_messages AS dmes " +
                    "ON id.dialog_id = dmes.dialog_id " +
                    "WHERE dm.user_id = @userId " +
                    "AND id.abstract_scope_id = @abstractScopeId ";

        var result = await connection.QueryAsync<GroupObjectDialogMessageOutput>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<GroupObjectDialogMessageOutput>> GetObjectDialogMessagesAsync(
        IEnumerable<long> objectIds)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@abstractScopeIds", objectIds.AsList());

        var query = "SELECT dmes.message_id, " +
                    "dmes.message, " +
                    "id.dialog_id, " +
                    "TO_CHAR(dmes.created_at, 'dd.MM.yyyy HH24:MI'), " +
                    "dmes.created_by, " +
                    "id.dialog_name AS label, " +
                    "id.abstract_scope_id AS ObjectId, " +
                    "dmes.is_my_message " +
                    "FROM communications.main_info_dialogs AS id " +
                    "INNER JOIN communications.dialog_members AS dm " +
                    "ON id.dialog_id = dm.dialog_id " +
                    "LEFT JOIN communications.dialog_messages AS dmes " +
                    "ON id.dialog_id = dmes.dialog_id " +
                    "WHERE id.abstract_scope_id = ANY (@abstractScopeIds) ";

        var result = await connection.QueryAsync<GroupObjectDialogMessageOutput>(query, parameters);

        return result;
    }
}