using LeokaEstetica.Platform.Base.Models.IntegrationEvents.ScrumMasterAi;

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
    /// <param name="token">Токен пользователя.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Событие с данными.</returns>
    public static ScrumMasterAiMessageEvent CreateScrumMasterAiMessageEvent(string? message, string? token,
        long userId)
    {
        return new ScrumMasterAiMessageEvent(message, token, userId);
    }
}