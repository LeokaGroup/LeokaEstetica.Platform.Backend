using LeokaEstetica.Platform.Models.Dto.Chat.Output;
using LeokaEstetica.Platform.Models.Entities.Communication;

namespace LeokaEstetica.Platform.Base.Abstractions.Repositories.Chat;

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
    /// Метод получает диалог по участнику и проекту.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Id диалога.</returns>
    Task<long> GetDialogMembersAsync(long userId, long projectId);

    /// <summary>
    /// Метод создаст новый диалог.
    /// </summary>
    /// <param name="dialogName">Название диалога.</param>
    /// <param name="dateCreated">Дата создания диалога.</param>
    /// <param name="isScrumMasterAi">Признак создания диалога для нейросети.</param>
    /// <returns>Id добавленного диалога.</returns>
    Task<long> CreateDialogAsync(string dialogName, DateTime dateCreated, bool isScrumMasterAi);

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
    /// Метод проверит существование диалога по участникам диалога.
    /// </summary>
    /// <param name="userId">Id пользователя (не владелец).</param>
    /// <param name="ownerId">Id владельца проекта.</param>
    /// <returns>Флаг проверки.</returns>
    Task<long?> CheckDialogAsync(long userId, long ownerId);

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

    /// <summary>
    /// Метод получит все диалоги.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectId">Id проекта. Если не передан, то получает все диалоги пользователя.</param>
    /// <returns>Список диалогов.</returns>
    Task<List<DialogOutput>> GetDialogsAsync(long userId, long? projectId = null);
    
    /// <summary>
    /// Метод получит все диалоги.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="objectId">Id проекта или вакансии. Может не указываться.</param>
    /// <returns>Список диалогов.</returns>
    Task<List<ScrumMasterAiNetworkDialogOutput>> GetDialogsScrumMasterAiAsync(long userId, long? objectId);

    /// <summary>
    /// Метод находит последнее сообщение диалога.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <returns>Последнее сообщение.</returns>
    Task<string> GetLastMessageAsync(long dialogId);

    /// <summary>
    /// Метод сохраняет сообщение.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="dialogId">Id диалога.</param>
    /// <param name="dateCreated">Дата записи сообщения.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="isMyMessage">Флаг принадлежности сообщения пользователю, который пишет сообщение.</param>
    Task SaveMessageAsync(string message, long dialogId, DateTime dateCreated, long userId, bool isMyMessage);

    /// <summary>
    /// Метод получает участников диалога.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <returns>Список участников диалога.</returns>
    Task<ICollection<DialogMemberEntity>> GetDialogMembersByDialogIdAsync(long dialogId);
    
    /// <summary>
    /// Метод получит все диалоги для профиля пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список диалогов.</returns>
    Task<List<ProfileDialogOutput>> GetProfileDialogsAsync(long userId);

    /// <summary>
    /// Метод устанавливает связь между проектом и диалогом.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <param name="projectId">Id проекта.</param>
    Task SetReferenceProjectDialogAsync(long dialogId, long projectId);

    /// <summary>
    /// Метод получает Id проекта Id диалога.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <returns>Id проекта.</returns>
    Task<long> GetDialogProjectIdByDialogIdAsync(long dialogId);
}