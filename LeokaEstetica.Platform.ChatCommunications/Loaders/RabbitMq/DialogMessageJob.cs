using System.Text;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Base.Models.IntegrationEvents.Communications;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Database.Abstractions.Communications;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using Newtonsoft.Json;
using Quartz;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LeokaEstetica.Platform.Communications.Loaders.RabbitMq;

/// <summary>
/// Класс джобы сообщений диалогов.
/// </summary>
[DisallowConcurrentExecution]
internal sealed class DialogMessageJob : IJob
{
    private IModel? _channel;
    private readonly ILogger<DialogMessageJob> _logger;
    private readonly IGlobalConfigRepository _globalConfigRepository;
    private readonly IDiscordService _discordService;
    private readonly IAbstractGroupDialogRepository _abstractGroupDialogRepository;

    /// <summary>
    /// Название очереди.
    /// </summary>
    private readonly string _queueName = string.Empty;

    /// <summary>
    /// Счетчик кол-ва подключений во избежание дублей подключений.
    /// </summary>
    private static uint _counter;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфига.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="abstractGroupDialogRepository">Репозиторий сообщений.</param>
    public DialogMessageJob(ILogger<DialogMessageJob> logger,
        IGlobalConfigRepository globalConfigRepository,
        IDiscordService discordService,
        IAbstractGroupDialogRepository abstractGroupDialogRepository)
    {
        _logger = logger;
        _globalConfigRepository = globalConfigRepository;
        _discordService = discordService;
        _abstractGroupDialogRepository = abstractGroupDialogRepository;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод запускает логику фоновой задачи.
    /// </summary>
    /// <param name="context">Выполняемый контекст джобы.</param>
    public async Task Execute(IJobExecutionContext context)
    {
        var isEnabledJob = await _globalConfigRepository
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.JobsMode.DIALOG_MESSAGES_JOB_MODE_ENABLED);

        if (!isEnabledJob)
        {
            return;
        }

        var dataMap = context.JobDetail.JobDataMap;

        var connection = new ConnectionFactory
        {
            HostName = dataMap.GetString("RabbitMq:HostName"),
            Password = dataMap.GetString("RabbitMq:Password"),
            UserName = dataMap.GetString("RabbitMq:UserName"),
            DispatchConsumersAsync = true,
            Port = AmqpTcpEndpoint.UseDefaultPort,
            VirtualHost = dataMap.GetString("RabbitMq:VirtualHost"),
            ContinuationTimeout = new TimeSpan(0, 0, 10, 0)
        };

        var flags = QueueTypeEnum.DialogMessages | QueueTypeEnum.DialogMessages;

        // Если кол-во подключений уже больше 1, то не будем плодить их,
        // а в рамках одного подключения будем работать с очередью.
        if (_counter < 1)
        {
            try
            {
                var connection1 = connection.CreateConnection();
                _channel = connection1.CreateModel();
                _channel.QueueDeclare(
                    queue: _queueName.CreateQueueDeclareNameFactory(dataMap.GetString("Environment")!, flags),
                    durable: false, exclusive: false, autoDelete: false, arguments: null);
            }

            catch (Exception ex)
            {
                await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);

                _logger.LogError(ex, ex.Message);
                throw;
            }

            _counter++;
        }

        // Если канал не был создан, то не будем дергать память.
        if (_channel is not null)
        {
            await ProcessMessagesAsync();
        }

        await Task.CompletedTask;
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод обрабатывает сообщения из очереди чата.
    /// </summary>
    private async Task ProcessMessagesAsync()
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            var logger = _logger; // Вне скоупа логгер не пишет логи, поэтому должны иметь внутренний логгер тут.
            logger.LogInformation("Начали обработку сообщения из очереди заказов...");

            // Если очередь не пуста, то будем парсить результат.
            if (!ea.Body.IsEmpty)
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var messageEvent = JsonConvert.DeserializeObject<DialogMessageEvent>(message);
                
                if (messageEvent is null)
                {
                    throw new InvalidOperationException("Событие не содержит нужных данных. " +
                                                        $"Получили сообщение из очереди сообщений чата: {message}");
                }
                
                // Добавляем сообщение в БД.
                var addedMessage = await _abstractGroupDialogRepository.SaveMessageAsync(messageEvent.Message,
                    messageEvent.CreatedBy, messageEvent.DialogId);

                if (addedMessage is null)
                {
                    var ex = new InvalidOperationException(
                        "Ошибка добавления сообщения в БД. " +
                        $"MessageEvent: {JsonConvert.SerializeObject(messageEvent)}.");

                    logger.LogError(ex, ex.Message);

                    await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
                }

                logger.LogInformation("Закончили обработку сообщения из очереди заказов...");

                await Task.Yield();
            }

            _channel.BasicConsume(_queueName, false, consumer);

            await Task.CompletedTask;
        };
    }

    #endregion
}