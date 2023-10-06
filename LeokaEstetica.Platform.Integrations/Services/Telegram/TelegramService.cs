using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Integrations.Abstractions.Telegram;
using LeokaEstetica.Platform.Models.Dto.Output.Integration.Telegram;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Integrations.Services.Telegram;

/// <summary>
/// Класс реализует методы сервиса телеграма.
/// </summary>
internal sealed class TelegramService : ITelegramService
{
    private readonly IGlobalConfigRepository _globalConfigRepository;
    private readonly ILogger<TelegramService> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="globalConfigRepository">Репозиторий глобал конфига.</param>
    /// <param name="logger">Сервис логгера.</param>
    public TelegramService(IGlobalConfigRepository globalConfigRepository,
        ILogger<TelegramService> logger)
    {
        _globalConfigRepository = globalConfigRepository;
        _logger = logger;
    }

    /// <summary>
    /// Метод создает ссылку для приглашения пользователя в канал уведомлений телеграмма.
    /// </summary>
    /// <returns>Строка приглашения.</returns>
    public async Task<CreateInviteLInkOutput> CreateNotificationsChanelInviteLinkAsync()
    {
        try
        {
            var result = new CreateInviteLInkOutput
            {
                Url = await _globalConfigRepository.GetValueByKeyAsync<string>(GlobalConfigKeys.Integrations.Telegram
                    .NOTIFICATIONS_BOT_INVITE)
            };

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}