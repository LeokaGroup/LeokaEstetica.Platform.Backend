using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Chat;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Models.Dto.Chat.Output;
using LeokaEstetica.Platform.Models.Entities.Communication;
using LeokaEstetica.Platform.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Enum = LeokaEstetica.Platform.Models.Enums.Enum;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Base.Repositories.Chat;

/// <summary>
/// Класс реализует методы репозитория чата.
/// </summary>
internal sealed class ChatRepository : BaseRepository, IChatRepository
{
    private readonly PgContext _pgContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public ChatRepository(PgContext pgContext, IConnectionProvider connectionProvider) 
        : base(connectionProvider)
    {
        _pgContext = pgContext;
    }

    #region Публичные методы.

    /// <inheritdoc />
    public async Task<long> GetDialogByUserIdAsync(long userId, bool isObjectTypeDialogAi)
    {
        long dialogId;
        
        if (!isObjectTypeDialogAi)
        {
            // TODO: Переписать на Dapper.
            dialogId = await _pgContext.DialogMembers
                .Where(dm => dm.UserId == userId)
                .Select(dm => dm.DialogId)
                .FirstOrDefaultAsync();
            
            return dialogId;
        }

        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId);

        var query = "SELECT dialog_id " +
                    "FROM ai.scrum_master_ai_dialog_members " +
                    "WHERE user_id = @userId";

        dialogId = await connection.QueryFirstOrDefaultAsync<long>(query, parameters);

        return dialogId;
    }

    /// <inheritdoc />
    public async Task<long> GetDialogMembersAsync(long userId, long? objectId, bool isObjectTypeDialogAi)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        string? query = null;
        var parameters = new DynamicParameters();

        if (!isObjectTypeDialogAi)
        {
            parameters.Add("@userId", userId);
            parameters.Add("@projectId", objectId!.Value);

            query = "SELECT dm.\"DialogId\" " +
                        "FROM \"Communications\".\"MainInfoDialogs\" AS d " +
                        "INNER JOIN \"Communications\".\"DialogMembers\" AS dm " +
                        "ON d.\"DialogId\" = dm.\"DialogId\" " +
                        "WHERE d.\"ProjectId\" = @projectId " +
                        "AND dm.\"UserId\" = @userId";
        }

        if (isObjectTypeDialogAi)
        {
            parameters.Add("@userId", userId);

            query = "SELECT dm.dialog_id " +
                    "FROM ai.scrum_master_ai_main_info_dialogs AS d " +
                    "INNER JOIN ai.scrum_master_ai_dialog_members AS dm " +
                    "ON dm.dialog_id = d.dialog_id " +
                    "WHERE dm.user_id = dm.user_id = @userId ";
            
            if (objectId.HasValue)
            {
                parameters.Add("@objectId", objectId.Value);
                query += "AND d.objectId = @objectId";
            }
        }

        var result = await connection.QueryFirstOrDefaultAsync<long>(query!, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task<long?> CreateDialogAsync(string dialogName, DateTime dateCreated, bool isScrumMasterAi)
    {
        long? dialogId = null;
        
        if (!isScrumMasterAi)
        {
            // TODO: Переписать на Dapper.
            var dialog = new MainInfoDialogEntity
            {
                DialogName = dialogName,
                Created = dateCreated
            };
            await _pgContext.Dialogs.AddAsync(dialog);
            await _pgContext.SaveChangesAsync();

            dialogId = dialog.DialogId;
        }

        if (isScrumMasterAi)
        {
            using var connection = await ConnectionProvider.GetConnectionAsync();
            
            var parameters = new DynamicParameters();
            parameters.Add("@dialogName", dialogName);
            parameters.Add("@created", dateCreated);
            parameters.Add("@objectType", new Enum(DiscussionTypeEnum.ObjectTypeDialogAi));

            var query = "INSERT INTO ai.scrum_master_ai_main_info_dialogs (dialog_name, created, object_type) " +
                        "VALUES (@dialogName, @created, @objectType)";

            dialogId = await connection.ExecuteScalarAsync<long?>(query, parameters);
        }

        return dialogId;
    }

    /// <inheritdoc />
    public async Task AddDialogMembersAsync(long userId, long ownerId, long newDialogId, bool isScrumMasterAi)
    {
        if (!isScrumMasterAi)
        {
            await _pgContext.DialogMembers.AddRangeAsync(
                new DialogMemberEntity
                {
                    DialogId = newDialogId,
                    UserId = userId,
                    Joined = DateTime.UtcNow
                },
                new DialogMemberEntity
                {
                    DialogId = newDialogId,
                    UserId = ownerId,
                    Joined = DateTime.UtcNow
                });
            await _pgContext.SaveChangesAsync();
        }

        if (isScrumMasterAi)
        {
            using var connection = await ConnectionProvider.GetConnectionAsync();

            var tempParams1 = new DynamicParameters();
            tempParams1.Add("@dialogId", newDialogId);
            tempParams1.Add("@userId", userId);
            tempParams1.Add("@joined", DateTime.UtcNow);
            
            var tempParams2 = new DynamicParameters();
            tempParams2.Add("@dialogId", newDialogId);
            tempParams2.Add("@userId", -1);
            tempParams2.Add("@joined", DateTime.UtcNow);
            
            var parameters = new List<DynamicParameters> { tempParams1, tempParams2 };

            var query = "INSERT INTO ai.scrum_master_ai_dialog_members (joined, dialog_id, user_id) " +
                        "VALUES (@joined, @dialogId, @userId)";

            await connection.ExecuteAsync(query, parameters);
        }
    }

    /// <inheritdoc />
    public async Task<bool> CheckDialogAsync(long dialogId, bool isScrumMasterAi)
    {
        if (!isScrumMasterAi)
        {
            var isDialog = await _pgContext.Dialogs
                .FirstOrDefaultAsync(d => d.DialogId == dialogId);
                
            return isDialog != null;
        }

        if (isScrumMasterAi)
        {
            using var connection = await ConnectionProvider.GetConnectionAsync();
            
            var parameters = new DynamicParameters();
            parameters.Add("@dialogId", dialogId);

            var query = "SELECT EXISTS (" +
                        "SELECT dialog_id " +
                        "FROM ai.scrum_master_ai_main_info_dialogs " +
                        "WHERE dialog_id = @dialogId)";

            var result = await connection.ExecuteScalarAsync<bool>(query, parameters);

            return result;
        }

        return false;
    }

    /// <inheritdoc />
    public async Task<long?> CheckDialogAsync(long userId, long ownerId)
    {
        var dialogId = await _pgContext.DialogMembers
            .Where(dm => dm.UserId == userId && dm.UserId == ownerId)
            .Select(dm => dm.DialogId)
            .FirstOrDefaultAsync();
        
        var isDialog = await _pgContext.Dialogs
            .FirstOrDefaultAsync(d => d.DialogId == dialogId);

        return isDialog?.DialogId;
    }

    /// <inheritdoc />
    public async Task<List<DialogMessageEntity>> GetDialogMessagesAsync(long dialogId, bool isScrumMasterAi)
    {
        List<DialogMessageEntity>? result = null;

        if (!isScrumMasterAi)
        {
            result = await _pgContext.DialogMessages
                .Where(d => d.DialogId == dialogId)
                .OrderBy(m => m.Created)
                .Select(m => new DialogMessageEntity
                {
                    DialogId = m.DialogId,
                    Message = m.Message,
                    Created = m.Created,
                    UserId = m.UserId,
                    IsMyMessage = m.IsMyMessage,
                    MessageId = m.MessageId
                })
                .ToListAsync();
        }

        if (isScrumMasterAi)
        {
            using var connection = await ConnectionProvider.GetConnectionAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@dialogId", dialogId);

            var query = "SELECT message_id, message, created, dialog_id, user_id, is_my_message " +
                        "FROM ai.scrum_master_ai_dialog_messages " +
                        "WHERE dialog_id = @dialogId";

            result = (await connection.QueryAsync<DialogMessageEntity>(query, parameters)).AsList();
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<List<long>> GetDialogMembersAsync(long dialogId, bool isScrumMasterAi)
    {
        List<long>? result = null;
        
        if (!isScrumMasterAi)
        {
            result = await _pgContext.DialogMembers
                .Where(d => d.DialogId == dialogId)
                .Select(d => d.UserId)
                .ToListAsync();
        }

        if (isScrumMasterAi)
        {
            using var connection = await ConnectionProvider.GetConnectionAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@dialogId", dialogId);

            var query = "SELECT user_id " +
                        "FROM ai.scrum_master_ai_dialog_members " +
                        "WHERE dialog_id = @dialogId";

            result = (await connection.QueryAsync<long>(query, parameters)).AsList();
        }

        return result;
    }

    /// <summary>
    /// Метод получает дату начала диалога.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <returns>Дата начала диалога.</returns>
    public async Task<string> GetDialogStartDateAsync(long dialogId, bool isScrumMasterAi)
    {
        string? result = null;
        
        if (!isScrumMasterAi)
        {
            result = await _pgContext.Dialogs
                .Where(d => d.DialogId == dialogId)
                .Select(d => d.Created.ToString("dd.MM.yyyy HH:mm"))
                .FirstOrDefaultAsync();
        }

        if (isScrumMasterAi)
        {
            using var connection = await ConnectionProvider.GetConnectionAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@dialogId", dialogId);

            var query = "SELECT TO_CHAR(TO_TIMESTAMP(1444646136079 / 1000), 'dd-mm-yyyy HH24:MI') " +
                        "FROM ai.scrum_master_ai_main_info_dialogs " +
                        "WHERE dialog_id = @dialogId";

            result = await connection.QueryFirstOrDefaultAsync<string?>(query, parameters);
        }

        return result;
    }

    /// <summary>
    /// Метод получит все диалоги.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectId">Id проекта. Если не передан, то получает все диалоги пользователя.</param>
    /// <returns>Список диалогов.</returns>
    public async Task<List<DialogOutput>> GetDialogsAsync(long userId, long? projectId = null)
    {
        List<DialogOutput>? result = null;
        using var connection = await ConnectionProvider.GetConnectionAsync();

        if (projectId.HasValue)
        {
            // Тут используем Limit 1, так как иначе были дубли из-за участников. Нам только диалоги нужны.
            var query = "DO " +
                        "$$ " +
                        "DECLARE " +
                        "_project_owner_id BIGINT; " +
                        "_project_id       BIGINT; " +
                        "_cnt_dialog_messages BIGINT; " +
                        "BEGIN " +
                        $"_project_id = {projectId}; " +
                        "_project_owner_id = (SELECT \"UserId\" " +
                        "FROM \"Projects\".\"UserProjects\" " +
                        "WHERE \"ProjectId\" = _project_id); " +
                        "IF (_project_owner_id IS NULL) THEN " +
                        "RAISE EXCEPTION 'Владелец проекта не определен. ProjectId: --> %', _project_id; " +
                        "END IF; " +
                        "_cnt_dialog_messages = (SELECT COUNT(m.\"MessageId\") " +
                        "FROM \"Communications\".\"MainInfoDialogs\" AS d " +
                        "INNER JOIN \"Communications\".\"DialogMembers\" AS dm " +
                        "ON d.\"DialogId\" = dm.\"DialogId\" " +
                        "INNER JOIN \"Communications\".\"DialogMessages\" AS m " +
                        "ON dm.\"DialogId\" = m.\"DialogId\" " +
                        $"WHERE dm.\"UserId\" = ANY (ARRAY [{userId}, _project_owner_id]) " +
                        $"AND d.\"ProjectId\" = {projectId}); " +
                        "DROP TABLE IF EXISTS temp_dialogs; " +
                        "CREATE TEMP TABLE temp_dialogs " +
                        "( " +
                        "\"DialogId\"   BIGINT                   NOT NULL, " +
                        "\"DialogName\" VARCHAR(150)             NOT NULL, " +
                        "\"UserId\"     BIGINT                   NOT NULL, " +
                        "\"Created\"    TIMESTAMP WITH TIME ZONE NOT NULL, " +
                        "\"ProjectId\"  BIGINT                   NOT NULL " +
                        "); " +
                        "INSERT INTO temp_dialogs " +
                        "SELECT DISTINCT(dm.\"DialogId\"), mid.\"DialogName\", dm.\"UserId\", mid.\"Created\", " +
                        "mid.\"ProjectId\" " +
                        "FROM \"Communications\".\"MainInfoDialogs\" AS mid " +
                        "INNER JOIN \"Communications\".\"DialogMembers\" AS dm " +
                        "ON mid.\"DialogId\" = dm.\"DialogId\" " +
                        "WHERE mid.\"ProjectId\" = _project_id " +
                        $"AND dm.\"UserId\" = ANY (ARRAY [{userId}, _project_owner_id]) " +
                        "AND _cnt_dialog_messages > 0 " +
                        "LIMIT 1;" +
                        "END " +
                        "$$; " +
                        "SELECT * FROM temp_dialogs;";

            result = (await connection.QueryAsync<DialogOutput>(query)).AsList();
        }

        // TODO: Пока не используется. Доработать надо будет, когда допилим сообщения из профиля
        // TODO: (общения по разным проектам из одного места). Отложили, так как не было времени на это.
        // else
        // {
        //     var query = "DO " +
        //                 "$$ " +
        //                 "BEGIN " +
        //                 "DROP TABLE IF EXISTS temp_dialogs; " +
        //                 "CREATE TEMP TABLE temp_dialogs " +
        //                 "( " +
        //                 "\"DialogId\"   BIGINT                   NOT NULL, " +
        //                 "\"DialogName\" VARCHAR(150)             NOT NULL, " +
        //                 "\"UserId\"     BIGINT                   NOT NULL, " +
        //                 "\"Created\"    TIMESTAMP WITH TIME ZONE NOT NULL, " +
        //                 "\"ProjectId\"  BIGINT                   NOT NULL " +
        //                 "); " +
        //                 "INSERT INTO temp_dialogs " +
        //                 "SELECT dm.\"DialogId\", mid.\"DialogName\", dm.\"UserId\", mid.\"Created\", " +
        //                 "mid.\"ProjectId\" " +
        //                 "FROM \"Communications\".\"MainInfoDialogs\" AS mid " +
        //                 "INNER JOIN \"Communications\".\"DialogMembers\" AS dm " +
        //                 "ON mid.\"DialogId\" = dm.\"DialogId\" " +
        //                 $"WHERE dm.\"UserId\" = {userId}; " +
        //                 "END " +
        //                 "$$; " +
        //                 "SELECT * FROM temp_dialogs;";
        //
        //     result = (await connection.QueryAsync<DialogOutput>(query)).AsList();
        // }

        return result;
    }

    /// <inheritdoc/>
    public async Task<List<ScrumMasterAiNetworkDialogOutput>> GetDialogsScrumMasterAiAsync(long userId, long? objectId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var builderObjectCondition = new StringBuilder();

        if (objectId.HasValue)
        {
            builderObjectCondition.Append(@"WHERE mid.object_id = {objectId} ");
        }

        else
        {
            builderObjectCondition.Append("WHERE mid.object_id IS NULL ");
        }

         // Тут используем Limit 1, так как иначе были дубли из-за участников. Нам только диалоги нужны.
            var query = "DO " +
                        "$$ " +
                        "DECLARE " +
                        "_cnt_dialog_messages BIGINT; " +
                        "BEGIN " +
                        "_cnt_dialog_messages = (SELECT COUNT(m.message_id) " +
                        "FROM ai.scrum_master_ai_main_info_dialogs AS d " +
                        "INNER JOIN ai.scrum_master_ai_dialog_members AS dm ON d.dialog_id = dm.dialog_id " +
                        "INNER JOIN ai.scrum_master_ai_dialog_messages AS m ON dm.dialog_id = m.dialog_id " +
                        $"WHERE dm.user_id = ANY (ARRAY [{userId}, -1])); " +
                        "DROP TABLE IF EXISTS temp_dialogs; " +
                        "CREATE TEMP TABLE temp_dialogs " +
                        "( " +
                        "dialog_id   BIGINT                   NOT NULL, " +
                        "dialog_name VARCHAR(150)             NOT NULL, " +
                        "user_id     BIGINT                   NOT NULL, " +
                        "created     TIMESTAMP WITH TIME ZONE NOT NULL, " +
                        "object_id   BIGINT                   NOT NULL, " +
                        "object_type OBJECT_TYPE_DIALOG_AI    NOT NULL " +
                        "); " +
                        "INSERT INTO temp_dialogs " +
                        "SELECT DISTINCT(mid.dialog_id), mid.dialog_name, dm.user_id, mid.created, mid.object_id, " +
                        "mid.object_type " +
                        "FROM ai.scrum_master_ai_main_info_dialogs AS mid " +
                        "INNER JOIN ai.scrum_master_ai_dialog_members AS dm ON mid.dialog_id = dm.dialog_id " +
                        builderObjectCondition +
                        $"AND dm.user_id = ANY (ARRAY [{userId}, -1]) " +
                        "AND _cnt_dialog_messages > 0 " +
                        "LIMIT 1; " +
                        "END " +
                        "$$; " +
                        "SELECT * FROM temp_dialogs;";

            var result = (await connection.QueryAsync<ScrumMasterAiNetworkDialogOutput>(query)).AsList();

        return result;
    }

    /// <summary>
    /// Метод находит последнее сообщение диалога.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <returns>Последнее сообщение.</returns>
    public async Task<string> GetLastMessageAsync(long dialogId)
    {
        var lastMessage = await _pgContext.DialogMessages
            .Where(d => d.DialogId == dialogId)
            .OrderBy(o => o.Created)
            .Select(m => m.Message)
            .LastOrDefaultAsync();

        return lastMessage;
    }

    /// <inheritdoc/>
    public async Task SaveMessageAsync(string message, long dialogId, DateTime dateCreated, long userId,
        bool isMyMessage, bool isScrumMasterAi)
    {
        if (!isScrumMasterAi)
        {
            await _pgContext.DialogMessages.AddAsync(new DialogMessageEntity
            {
                Message = message,
                DialogId = dialogId,
                Created = dateCreated,
                UserId = userId,
                IsMyMessage = isMyMessage
            });

            await _pgContext.SaveChangesAsync();
        }

        if (isScrumMasterAi)
        {
            using var connection = await ConnectionProvider.GetConnectionAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@message", message);
            parameters.Add("@dialogId", dialogId);
            parameters.Add("@userId", userId);
            parameters.Add("@isMyMessage", isMyMessage);
            parameters.Add("@dateCreated", dateCreated);

            var query = "INSERT INTO ai.scrum_master_ai_dialog_messages (message, dialog_id, user_id, is_my_message," +
                        " created) " +
                        "VALUES (@message, @dialogId, @userId, @isMyMessage, @dateCreated)";

            await connection.ExecuteAsync(query, parameters);
        }
    }

    /// <summary>
    /// Метод получает участников диалога.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <returns>Список участников диалога.</returns>
    public async Task<ICollection<DialogMemberEntity>> GetDialogMembersByDialogIdAsync(long dialogId)
    {
        var result = await _pgContext.DialogMembers
            .Where(d => d.DialogId == dialogId)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод устанавливает связь между проектом и диалогом.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <param name="projectId">Id проекта.</param>
    public async Task SetReferenceProjectDialogAsync(long dialogId, long projectId)
    {
        var dialog = await _pgContext.Dialogs
            .FirstOrDefaultAsync(d => d.DialogId == dialogId);

        if (dialog is null)
        {
            throw new InvalidOperationException($"Диалог не найден. DialogId: {dialogId}");
        }

        // Не надо нагружать базу, если уже привязан к проекту.
        if (dialog.ProjectId == projectId)
        {
            return;
        }

        dialog.ProjectId = projectId;

        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод получит все диалоги для профиля пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список диалогов.</returns>
    public async Task<List<ProfileDialogOutput>> GetProfileDialogsAsync(long userId)
    {
        var result = await (from dm in _pgContext.DialogMembers
                join d in _pgContext.Dialogs
                    on dm.DialogId
                    equals d.DialogId
                where dm.UserId == userId
                      && d.DialogMessages.Any()
                select new ProfileDialogOutput
                {
                    DialogId = dm.DialogId,
                    DialogName = d.DialogName,
                    UserId = dm.UserId,
                    Created = d.Created.ToString(CultureInfo.CurrentCulture),
                    ProjectName = _pgContext.UserProjects
                        .Where(p => p.ProjectId == d.ProjectId)
                        .Select(p => p.ProjectName)
                        .FirstOrDefault(),
                    ProjectId = _pgContext.UserProjects
                        .Where(p => p.ProjectId == d.ProjectId 
                                    && d.ProjectId != null)
                        .Select(p => p.ProjectId)
                        .FirstOrDefault(),
                })
            .ToListAsync();

        return result;
    }
    
    /// <summary>
    /// Метод получает Id проекта Id диалога.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <returns>Id проекта.</returns>
    public async Task<long> GetDialogProjectIdByDialogIdAsync(long dialogId)
    {
        var result = await _pgContext.Dialogs
            .Where(d => d.DialogId == dialogId)
            .Select(d => d.ProjectId)
            .FirstOrDefaultAsync();

        if (!result.HasValue)
        {
            throw new InvalidOperationException($"Не удалось получить данные диалога. DialogId: {dialogId}");
        }

        return result.Value;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}