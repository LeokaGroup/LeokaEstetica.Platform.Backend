using LeokaEstetica.Platform.Messaging.Models.Chat.Output;
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
}