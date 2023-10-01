using LeokaEstetica.Platform.Models.Dto.Output.Integration.Telegram;

namespace LeokaEstetica.Platform.Integrations.Abstractions.Telegram;

/// <summary>
/// Абстракция сервиса телеграма.
/// </summary>
public interface ITelegramService
{
    /// <summary>
    /// Метод создает ссылку для приглашения пользователя в канал уведомлений телеграмма.
    /// </summary>
    /// <returns>Строка приглашения.</returns>
    Task<CreateInviteLInkOutput> CreateNotificationsChanelInviteLinkAsync();
}