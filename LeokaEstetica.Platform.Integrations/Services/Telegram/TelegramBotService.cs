using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Integrations.Abstractions.Telegram;
using LeokaEstetica.Platform.Integrations.Enums;
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
    private static string _logBotToken;
    private static string _notificationsBot;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="configuration">Конфигурация.</param>
    public TelegramBotService(ILogger<TelegramBotService> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _chatId = configuration["LogBot:ChatId"];
        _logBotToken = configuration["LogBot:Token"];
        _notificationsBot = configuration["NotificationsBot:Token"];
    }

    #region Публичные методы.

    /// <summary>
    /// Метод отправляет информацию об ошибке в канал телеграма.
    /// </summary>
    /// <param name="errorMessage">Вся инфолрмация об исключении.</param>
    public async Task SendErrorMessageAsync(string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            return;
        }
            
        var botClient = new TelegramBotClient(_logBotToken);
        
        try
        {
            var bot = await botClient.GetMeAsync();

            if (bot.IsBot)
            {
                await botClient.SendTextMessageAsync(_chatId, errorMessage);
            }
        }
        
        catch (Exception ex1)
        {
            _logger.LogCritical(ex1,
                "Ошибка при отправке исключения в канал. Не волнуйтесь, ошибку сохранили. Смотреть БД.");
            
            _logger.LogInformation("Повторная попытка отправить ошибку в чат телеграм.");

            try
            {
                await botClient.SendTextMessageAsync(_chatId, errorMessage);
            }
            
            catch (Exception ex2)
            {
                _logger.LogCritical(ex2, "Повторная отправка ошибки не удалась.");
                throw;
            }
        }
    }

    /// <summary>
    /// Метод отправляет уведомление в чат о созданной вакансии, проекте.
    /// </summary>
    /// <param name="objectType">Тип объекта (вакансия, проект).</param>
    /// <param name="objectName">Название объекта (проекта, вакансии).</param>
    public async Task SendNotificationCreatedObjectAsync(ObjectTypeEnum objectType, string objectName)
    {
        var botClient = new TelegramBotClient(_notificationsBot);
        var notifyMessage = string.Empty;

        try
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                var ex = new InvalidOperationException(
                    "Название объекта (проекта, вакансии) не заполнено. Невозможно отправить уведомление в канал.");
                throw ex;
            }

            if (objectType.HasFlag(ObjectTypeEnum.Project))
            {
                notifyMessage = $"Создан новый проект \"{objectName}\".";
            }
        
            else if (objectType.HasFlag(ObjectTypeEnum.Vacancy))
            {
                notifyMessage = $"Создана новая вакансия \"{objectName}\".";
            }
            
            if (new[] {"Development", "Staging"}.Contains(_configuration["Environment"]))
            {
                await botClient.SendTextMessageAsync(_configuration["NotificationsBot:ChatIdDevelopTest"],
                    notifyMessage);
            }
        
            else
            {
                await botClient.SendTextMessageAsync(_configuration["NotificationsBot:ChatId"], notifyMessage);
            }
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            
            _logger.LogInformation("Повторная попытка отправить уведомление в чат телеграм.");

            try
            {
                if (new[] {"Development", "Staging"}.Contains(_configuration["Environment"]))
                {
                    await botClient.SendTextMessageAsync(_configuration["NotificationsBot:ChatIdDevelopTest"],
                        notifyMessage);
                }
        
                else
                {
                    await botClient.SendTextMessageAsync(_configuration["NotificationsBot:ChatId"], notifyMessage);
                }
            }
            
            catch (Exception ex2)
            {
                _logger.LogError(ex2, "Повторная отправка уведомления не удалась.");
                throw;
            }
        }
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}