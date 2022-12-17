using LeokaEstetica.Platform.Messaging.Models.Chat.Output;
using LeokaEstetica.Platform.Models.Dto.Chat.Output;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Messaging.Abstractions.Chat;

/// <summary>
/// Абстракция сервиса чата.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Метод получает диалог или создает новый и возвращает его.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <param name="discussionType">Тип объекта обсуждения.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="discussionTypeId">Id предмета обсуждения (Id проекта или вакансии).</param>
    /// <returns>Данные диалога.</returns>
    Task<DialogResultOutput> GetDialogAsync(long? dialogId, DiscussionTypeEnum discussionType, string account,
        long discussionTypeId);

    /// <summary>
    /// Метод получает список диалогов.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список диалогов.</returns>
    Task<IEnumerable<DialogOutput>> GetDialogsAsync(string account);

    /// <summary>
    /// Метод создает диалог для написания владельцу проекта.
    /// Если такой диалог уже создан с текущим юзером и владельцем проекта,
    /// то ничего не происходит и диалог считается пустым для начала общения.
    /// <param name="discussionType">Тип объекта обсуждения.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="discussionTypeId">Id предмета обсуждения (Id проекта или вакансии).</param>
    /// <returns>Данные диалога.</returns>
    Task<DialogResultOutput> WriteProjectDialogOwnerAsync(DiscussionTypeEnum discussionType, string account, long discussionTypeId);
}