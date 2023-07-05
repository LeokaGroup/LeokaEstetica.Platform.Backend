namespace LeokaEstetica.Platform.Integrations.Abstractions.Telegram;

/// <summary>
/// Абстракция сервиса телеграм бота.
/// </summary>
public interface ITelegramBotService
{
    /// <summary>
    /// Метод отправляет информацию об ошибке в канал телеграма.
    /// </summary>
    /// <param name="errorMessage">Вся инфолрмация об исключении.</param>
    Task SendErrorMessageAsync(string errorMessage);
}