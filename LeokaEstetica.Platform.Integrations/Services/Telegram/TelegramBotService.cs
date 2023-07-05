using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Integrations.Abstractions.Telegram;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Integrations.Services.Telegram;

/// <summary>
/// Класс реализует методы сервиса телеграм бота.
/// </summary>
internal sealed class TelegramBotService : ITelegramBotService
{
    private readonly ILogger<TelegramBotService> _logger;
    private static string _chatId;
    private static string _botToken;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="configuration">Конфигурация.</param>
    public TelegramBotService(ILogger<TelegramBotService> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _chatId = configuration["LogBot:ChatId"];
        _botToken = configuration["LogBot:Token"];
    }

    /// <summary>
    /// Метод отправляет информацию об ошибке в канал телеграма.
    /// </summary>
    /// <param name="errorMessage">Вся инфолрмация об исключении.</param>
    public async Task SendErrorMessageAsync(string errorMessage)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                return;
            }
            
            var botClient = new TelegramBotClient(_botToken);

            var bot = await botClient.GetMeAsync();

            if (bot.IsBot)
            {
                await botClient.SendTextMessageAsync(_chatId, errorMessage);
            }
        }
        
        catch (Exception ex)
        {
            _logger.LogCritical(ex,
                "Ошибка при отправке исключения в канал. Не волнуйтесь, ошибку сохранили. Смотреть БД.");
            throw;
        }
    }
}