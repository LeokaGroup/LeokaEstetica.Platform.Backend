using System.Runtime.CompilerServices;
using System.Text;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Database.Abstractions.Config;
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
    private readonly IGlobalConfigRepository _globalConfigRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="configuration">Конфигурация.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфига.</param>
    public TelegramBotService(ILogger<TelegramBotService> logger,
        IConfiguration configuration,
        IGlobalConfigRepository globalConfigRepository)
    {
        _logger = logger;
        _configuration = configuration;
        _globalConfigRepository = globalConfigRepository;
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
    /// <param name="objectDescription">Описание объекта (проекта, вакансии).</param>
    /// <param name="objectId">Id объекта (проекта, вакансии).</param>
    public async Task SendNotificationCreatedObjectAsync(ObjectTypeEnum objectType, string objectName,
        string objectDescription, long objectId)
    {
        var botClient = new TelegramBotClient(_notificationsBot);
        var notifyMessage = new StringBuilder();

        try
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                var ex = new InvalidOperationException(
                    "Название объекта (проекта, вакансии) не заполнено. Невозможно отправить уведомление в канал.");
                throw ex;
            }

            var objectLink = await _globalConfigRepository.GetValueByKeyAsync<string>(GlobalConfigKeys.Integrations
                .Telegram.NOTIFICATIONS_BOT_CREATED_OBJECT_LINK);

            if (objectType.HasFlag(ObjectTypeEnum.Project))
            {
                notifyMessage.AppendLine($"Создан новый проект: {objectName}.");
                notifyMessage.AppendLine(objectDescription);
                notifyMessage.AppendLine(string.Concat(objectLink, $"projects/project?projectId={objectId}&mode=view"));
            }
        
            else if (objectType.HasFlag(ObjectTypeEnum.Vacancy))
            {
                notifyMessage.AppendLine($"Создана новая вакансия: {objectName}.");
                notifyMessage.AppendLine(objectDescription);
                notifyMessage.AppendLine(string.Concat(objectLink, $"vacancies/vacancy?vacancyId={objectId}&mode=view"));
            }

            if (new[] {"Development", "Staging"}.Contains(_configuration["Environment"]))
            {
                await botClient.SendTextMessageAsync(_configuration["NotificationsBot:ChatIdDevelopTest"],
                    notifyMessage.ToString());
            }
        
            else
            {
                await botClient.SendTextMessageAsync(_configuration["NotificationsBot:ChatId"],
                    notifyMessage.ToString());
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
                        notifyMessage.ToString());
                }
        
                else
                {
                    await botClient.SendTextMessageAsync(_configuration["NotificationsBot:ChatId"],
                        notifyMessage.ToString());
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