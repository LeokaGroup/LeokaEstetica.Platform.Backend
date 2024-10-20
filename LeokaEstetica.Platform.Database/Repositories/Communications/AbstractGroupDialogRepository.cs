using System.Data;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Database.Abstractions.Communications;
using LeokaEstetica.Platform.Models.Dto.Communications.Output;

namespace LeokaEstetica.Platform.Database.Repositories.Communications;

/// <summary>
/// Класс реализует методы репозитория диалогов.
/// </summary>
internal sealed class AbstractGroupDialogRepository : BaseRepository, IAbstractGroupDialogRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="connectionProvider">Провайдер БД.</param>
    public AbstractGroupDialogRepository(IConnectionProvider connectionProvider) 
        : base(connectionProvider)
    {
    }

    #region Публичные методы.

    /// <inheritdoc />
    public async Task<GroupObjectDialogOutput?> CreateDialogAndAddDialogMembersAsync(IEnumerable<long> memberIds,
        string? dialogName)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        using var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            var addDialogParameters = new DynamicParameters();
            addDialogParameters.Add("@dialogName", dialogName);
            addDialogParameters.Add("@abstractScopeId", null);

            var addDialogQuery = "INSERT INTO communications.main_info_dialogs (dialog_name, abstract_scope_id) " +
                                 "VALUES (@dialogName, @abstractScopeId) " +
                                 "RETURNING dialog_id";

            var dialogId = await connection.ExecuteAsync(addDialogQuery, addDialogParameters);
            
            var addMembersParameters = new List<DynamicParameters>();

            foreach (var id in memberIds)
            {
                var tempParameters = new DynamicParameters();
                tempParameters.Add("@dialogId", dialogId);
                tempParameters.Add("@memberId", id);

                addMembersParameters.Add(tempParameters);
            }

            var addMembersQuery = "INSERT INTO communications.dialog_members (user_id, dialog_id) " +
                                  "VALUES (@memberId, @dialogId)";
            
            await connection.ExecuteAsync(addMembersQuery, addMembersParameters);
            
            transaction.Commit();
            
            var parameters = new DynamicParameters();
            parameters.Add("@dialogId", dialogId);

            var query = "SELECT dialog_id, " +
                        "dialog_name AS label, " +
                        "TO_CHAR(created_at, 'dd.MM.yyyy HH24:MI'), " +
                        "abstract_scope_id AS object_id " +
                        "FROM communications.main_info_dialogs " +
                        "WHERE dialog_id = @dialogId";

            var result = await connection.QuerySingleOrDefaultAsync<GroupObjectDialogOutput>(query, parameters);

            return result;
        }
        
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<GroupObjectDialogMessageOutput?> SaveMessageAsync(string? message, long createdBy, long dialogId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var addedMessageParameters = new DynamicParameters();
        addedMessageParameters.Add("@message", message);
        addedMessageParameters.Add("@createdBy", createdBy);
        addedMessageParameters.Add("@dialogId", dialogId);

        var addedMessageQuery = "INSERT INTO communications.dialog_messages (dialog_id, message, created_by) " +
                                "VALUES (@dialogId, @message, @createdBy) " +
                                "RETURNING message_id";

        var messageId = await connection.ExecuteScalarAsync<long>(addedMessageQuery, addedMessageParameters);
        
        var messageParameters = new DynamicParameters();
        messageParameters.Add("@messageId", messageId);
        messageParameters.Add("@createdBy", createdBy);

        var messageQuery = "SELECT message_id, " +
                           "dialog_id, " +
                           "message, " +
                           "created_at, " +
                           "created_by, " +
                           "(CASE " +
                           "WHEN created_by = @createdBy " +
                           "THEN TRUE " +
                           "ELSE FALSE END) AS is_my_message " +
                           "FROM communications.dialog_messages " +
                           "WHERE message_id = @messageId";

        var result = await connection.QuerySingleOrDefaultAsync<GroupObjectDialogMessageOutput>(messageQuery,
            messageParameters);

        return result;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}