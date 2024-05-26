using LeokaEstetica.Platform.Models.Dto.Chat.Output;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Base.Abstractions.Messaging.Chat;

/// <summary>
/// Абстракция сервиса чата.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// TODO: Возможно разделим на часть с нейросетями и часть обычных чатов.
    /// Метод получает диалог или создает новый и возвращает его.
    /// Метод используется и в обычных чатах и в чатах с нейросетями.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <param name="discussionType">Тип объекта обсуждения.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="discussionTypeId">Id предмета обсуждения (Id проекта или вакансии).</param>
    /// <param name="isManualNewDialog">Признак ручного создания диалога (по кнопке и тд).</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Данные диалога.</returns>
    Task<DialogResultOutput> GetDialogAsync(long? dialogId, DiscussionTypeEnum discussionType, string account,
        long? discussionTypeId, bool isManualNewDialog, string? token);

    /// <summary>
    /// Метод создает диалог для написания владельцу проекта.
    /// Если такой диалог уже создан с текущим юзером и владельцем проекта,
    /// то ничего не происходит и диалог считается пустым для начала общения.
    /// <param name="discussionType">Тип объекта обсуждения.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="discussionTypeId">Id предмета обсуждения (Id проекта или вакансии).</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Данные диалога.</returns>
    Task<DialogResultOutput> WriteProjectDialogOwnerAsync(DiscussionTypeEnum discussionType, string account,
        long? discussionTypeId, string token);

    /// <summary>
    /// Метод отправляет сообщение.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="dialogId">Id диалога.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <param name="isMyMessage">Флаг принадлежности сообщения пользователю, который пишет сообщение.</param>
    /// <param name="isScrumMasterAi">Признак диалога для нейросети.</param>
    /// <returns>Выходная модель.</returns>
    Task<DialogResultOutput> SendMessageAsync(string message, long dialogId, long userId, string token,
        bool isMyMessage, bool isScrumMasterAi);
    
    /// <summary>
    /// Метод получит все диалоги для профиля пользователя.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список диалогов.</returns>
    Task<List<ProfileDialogOutput>> GetProfileDialogsAsync(string account);
}