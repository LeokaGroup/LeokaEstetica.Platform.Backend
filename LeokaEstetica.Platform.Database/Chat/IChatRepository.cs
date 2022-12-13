using LeokaEstetica.Platform.Models.Entities.Communication;

namespace LeokaEstetica.Platform.Database.Chat;

/// <summary>
/// Абстракция репозитория чата.
/// </summary>
public interface IChatRepository
{
    /// <summary>
    /// Метод находит Id диалога в участниках диалога.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Id диалога.</returns>
    Task<long> GetDialogByUserIdAsync(long userId);

    /// <summary>
    /// Метод получает диалог, где есть и текущий пользователь и владелец предмета обсуждения.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="ownerId">Id владельца.</param>
    /// <returns>Id диалога.</returns>
    Task<long> GetDialogMembersAsync(long userId, long ownerId);

    /// <summary>
    /// Метод создаст новый диалог.
    /// </summary>
    /// <param name="dialogName">Название диалога.</param>
    /// <param name="dateCreated">Дата создания диалога.</param>
    /// <returns>Id добавленного диалога.</returns>
    Task<long> CreateDialogAsync(string dialogName, DateTime dateCreated);
    
    /// <summary>
    /// Метод добавит текущего пользователя и представителя/владельца к диалогу.
    /// </summary>
    /// <param name="userId">Id текущего пользователя.</param>
    /// <param name="ownerId">Id владельца.</param>
    /// <param name="newDialogId">Id нового диалога.</param>
    Task AddDialogMembersAsync(long userId, long ownerId, long newDialogId);
    
    /// <summary>
    /// Метод проверит существование диалога.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <returns>Флаг проверки.</returns>
    Task<bool> CheckDialogAsync(long dialogId);
    
    /// <summary>
    /// Метод получает список сообщений диалога.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <returns>Список сообщений.</returns>
    Task<List<DialogMessageEntity>> GetDialogMessagesAsync(long dialogId);
    
    /// <summary>
    /// Метод получает диалог, где есть и текущий пользователь и владелец предмета обсуждения.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <returns>Список Id участников диалога.</returns>
    Task<List<long>> GetDialogMembersAsync(long dialogId);

    /// <summary>
    /// Метод получает дату начала диалога.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <returns>Дата начала диалога.</returns>
    Task<string> GetDialogStartDateAsync(long dialogId);
}