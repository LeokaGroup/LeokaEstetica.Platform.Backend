using LeokaEstetica.Platform.Base.Models.IntegrationEvents.ScrumMasterAi;
using LeokaEstetica.Platform.Core.Enums;

namespace LeokaEstetica.Platform.Base.Factors;

/// <summary>
/// Класс факторки сообщений нейросети Scrum Master AI.
/// </summary>
public static class ScrumMasterAiMessageEventFactory
{
    /// <summary>
    /// Метод наполняет данными событие сообщений нейросети.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="connectionId">Id подключения сокетов.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="scrumMasterAiEventType">Тип ивента нейросети.</param>
    /// <param name="dialogId">Id диалога.</param>
    /// <returns>Событие с данными.</returns>
    public static ScrumMasterAiMessageEvent CreateScrumMasterAiMessageEvent(string? message, string? connectionId,
        long userId, ScrumMasterAiEventTypeEnum scrumMasterAiEventType, long dialogId)
    {
        return new ScrumMasterAiMessageEvent(message, connectionId, userId, scrumMasterAiEventType, dialogId);
    }
}