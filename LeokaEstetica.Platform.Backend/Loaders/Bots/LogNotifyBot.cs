using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LeokaEstetica.Platform.Backend.Loaders.Bots;

/// <summary>
/// Класс уведомлений телеграм бота.
/// </summary>
public static class LogNotifyBot
{
    /// <summary>
    /// Метод запускает бота.
    /// </summary>
    /// <param name="configuration">Конфигурация.</param>
    public static async Task RunAsync(IConfiguration configuration)
    {
        var botClient = new TelegramBotClient(configuration["LogBot:Token"]);

        using var cts = new CancellationTokenSource();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        botClient.StartReceiving(updateHandler: HandleUpdateAsync, pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions, cancellationToken: cts.Token);

        cts.Cancel();
        
        await Task.CompletedTask;
    }

    /// <summary>
    /// Метод обработки обновлений.
    /// </summary>
    /// <param name="botClient">Бот.</param>
    /// <param name="update">Обновление.</param>
    /// <param name="cancellationToken">Токен отмены задачи.</param>
    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    /// <summary>
    /// Метод обработки ошибок бота.
    /// </summary>
    /// <param name="botClient">Бот.</param>
    /// <param name="exception">Исключение.</param>
    /// <param name="cancellationToken">Токен отмены задачи.</param>
    private static async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}