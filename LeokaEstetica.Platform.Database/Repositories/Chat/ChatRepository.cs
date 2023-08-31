using System.Globalization;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Chat;
using LeokaEstetica.Platform.Models.Dto.Chat.Output;
using LeokaEstetica.Platform.Models.Entities.Communication;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Chat;

/// <summary>
/// Класс реализует методы репозитория чата.
/// </summary>
internal sealed class ChatRepository : IChatRepository
{
    private readonly PgContext _pgContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public ChatRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод находит Id диалога в участниках диалога.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Id диалога.</returns>
    public async Task<long> GetDialogByUserIdAsync(long userId)
    {
        var result = await _pgContext.DialogMembers
            .Where(dm => dm.UserId == userId)
            .Select(dm => dm.DialogId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает диалог по Id пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Id диалога.</returns>
    public async Task<long> GetDialogMembersByUserIdAsync(long userId)
    {
        var result = await _pgContext.DialogMembers
            .Where(dm => dm.UserId == userId)
            .Select(dm => dm.DialogId)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод создаст новый диалог.
    /// </summary>
    /// <param name="dialogName">Название диалога.</param>
    /// <param name="dateCreated">Дата создания диалога.</param>
    /// <returns>Id добавленного диалога.</returns>
    public async Task<long> CreateDialogAsync(string dialogName, DateTime dateCreated)
    {
        var dialog = new MainInfoDialogEntity
        {
            DialogName = dialogName,
            Created = dateCreated
        };
        await _pgContext.Dialogs.AddAsync(dialog);
        await _pgContext.SaveChangesAsync();

        return dialog.DialogId;
    }

    /// <summary>
    /// Метод добавит текущего пользователя и представителя/владельца к диалогу.
    /// </summary>
    /// <param name="userId">Id текущего пользователя.</param>
    /// <param name="ownerId">Id владельца.</param>
    /// <param name="newDialogId">Id нового диалога.</param>
    public async Task AddDialogMembersAsync(long userId, long ownerId, long newDialogId)
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

    /// <summary>
    /// Метод проверит существование диалога.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <returns>Флаг проверки.</returns>
    public async Task<bool> CheckDialogAsync(long dialogId)
    {
        var isDialog = await _pgContext.Dialogs
            .FirstOrDefaultAsync(d => d.DialogId == dialogId);

        return isDialog != null;
    }

    /// <summary>
    /// Метод проверит существование диалога по участникам диалога.
    /// </summary>
    /// <param name="userId">Id пользователя (не владелец).</param>
    /// <param name="ownerId">Id владельца проекта.</param>
    /// <returns>Флаг проверки.</returns>
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

    /// <summary>
    /// Метод получает список сообщений диалога.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <returns>Список сообщений.</returns>
    public async Task<List<DialogMessageEntity>> GetDialogMessagesAsync(long dialogId)
    {
        var result = await _pgContext.DialogMessages
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

        return result;
    }

    /// <summary>
    /// Метод получает диалог, где есть и текущий пользователь и владелец предмета обсуждения.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <returns>Список Id участников диалога.</returns>
    public async Task<List<long>> GetDialogMembersAsync(long dialogId)
    {
        var result = await _pgContext.DialogMembers
            .Where(d => d.DialogId == dialogId)
            .Select(d => d.UserId)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает дату начала диалога.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <returns>Дата начала диалога.</returns>
    public async Task<string> GetDialogStartDateAsync(long dialogId)
    {
        var result = await _pgContext.Dialogs
            .Where(d => d.DialogId == dialogId)
            .Select(d => d.Created.ToString("dd.MM.yyyy HH:mm"))
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получит все диалогы.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список диалогов.</returns>
    public async Task<List<DialogOutput>> GetDialogsAsync(long userId)
    {
        var result = await (from dm in _pgContext.DialogMembers
                join d in _pgContext.Dialogs
                    on dm.DialogId
                    equals d.DialogId
                where dm.UserId == userId 
                      && d.DialogMessages.Any()
                select new DialogOutput
                {
                    DialogId = dm.DialogId,
                    DialogName = d.DialogName,
                    UserId = dm.UserId,
                    Created = d.Created.ToString(CultureInfo.CurrentCulture)
                })
            .ToListAsync();

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

    /// <summary>
    /// Метод сохраняет сообщение.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="dialogId">Id диалога.</param>
    /// <param name="dateCreated">Дата записи сообщения.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="isMyMessage">Флаг принадлежности сообщения пользователю, который пишет сообщение.</param>
    public async Task SaveMessageAsync(string message, long dialogId, DateTime dateCreated, long userId,
        bool isMyMessage)
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

        dialog.ProjectId = projectId;

        await _pgContext.SaveChangesAsync();
    }

    #endregion

    #region Приватные

    

    #endregion
}