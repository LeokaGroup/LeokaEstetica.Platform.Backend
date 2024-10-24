using System.Text;
using FluentValidation;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Base.Models.IntegrationEvents.Communications;
using LeokaEstetica.Platform.Communications.Hubs;
using LeokaEstetica.Platform.Communications.Models;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Database.Abstractions.Communications;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Redis.Abstractions.Connection;
using Microsoft.AspNetCore.SignalR;
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
    private readonly IHubContext<CommunicationsHub> _communicationsHub;
    private readonly IConnectionService _connectionService;

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
    /// <param name="communicationsHub">Хаб коммуникаций.</param>
    /// <param name="connectionService">Сервис подключений к кэшу Redis.</param>
    public DialogMessageJob(ILogger<DialogMessageJob> logger,
        IGlobalConfigRepository globalConfigRepository,
        IDiscordService discordService,
        IAbstractGroupDialogRepository abstractGroupDialogRepository,
        IHubContext<CommunicationsHub> communicationsHub,
        IConnectionService connectionService)
    {
        _logger = logger;
        _globalConfigRepository = globalConfigRepository;
        _discordService = discordService;
        _abstractGroupDialogRepository = abstractGroupDialogRepository;
        _communicationsHub = communicationsHub;
        _connectionService = connectionService;
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
        
        // TODO: Проверить, не будет ли плодить подключения это?
        if (_counter == 1 && _channel is null)
        {
            var connection1 = connection.CreateConnection();
            _channel = connection1.CreateModel();
        
            _channel.QueueDeclare(queue: string.Empty.CreateQueueDeclareNameFactory(dataMap.GetString("Environment")!,
                flags), durable: false, exclusive: false, autoDelete: true, arguments: null);
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
            logger.LogInformation("Начали обработку сообщения из очереди сообщений чата...");

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
                    messageEvent.CreatedBy, messageEvent.DialogId, messageEvent.IsMyMessage);

                if (addedMessage is null)
                {
                    var ex = new InvalidOperationException(
                        "Ошибка добавления сообщения в БД. " +
                        $"MessageEvent: {JsonConvert.SerializeObject(messageEvent)}.");

                    logger.LogError(ex, ex.Message);

                    await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);

                    return;
                }

                var messageDto = new MessageDto
                {
                    Label = addedMessage.Label,
                    CreatedBy = addedMessage.CreatedBy,
                    DialogId = addedMessage.DialogId,
                    IsMyMessage = addedMessage.IsMyMessage,
                    UserCode = messageEvent.UserCode,
                    Module = messageEvent.Module
                };

                var errors = await ValidateMessageToFrontAsync(messageDto);

                // Не ломаем систему, но оставляем сообщение в очереди.
                if (errors.Count > 0)
                {
                    var ex = new AggregateException("Ошибки валидации сообщения при отправке фронту.", errors);
                    
                    logger.LogError(ex, ex.Message);

                    await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);

                    return;
                }
                
                var key = string.Concat(messageDto.UserCode + "_", messageDto.Module.ToString());
                var connection = await _connectionService.GetConnectionIdCacheAsync(key);

                if (string.IsNullOrWhiteSpace(connection?.ConnectionId))
                {
                    throw new InvalidOperationException(
                        "Ошибка получения подключения пользователя из Redis. " +
                        "Ошибка в джобе сообщений чата.");
                }
                
                // Отправляем сообщение фронту через хаб.
                await _communicationsHub.Clients
                    .Client(connection.ConnectionId)
                    .SendAsync("sendMessageToFront", messageDto)
                    .ConfigureAwait(false);

                logger.LogInformation("Закончили обработку сообщения из очереди сообщений чата...");
                
                if (_channel is null)
                {
                    await ThrowChannelExceptionAsync();
                    return;
                }
                
                // Подтверждаем сообщение, чтобы дропнуть его из очереди.
                _channel.BasicAck(ea.DeliveryTag, false);

                await Task.Yield();
            }
        };
        
        _channel.BasicConsume(_queueName, false, consumer);
        
        await Task.CompletedTask;
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод валидирует данные сообщения перед отправкой его на фронт.
    /// </summary>
    /// <param name="messageDto">Данные сообщения для валидации.</param>
    private async Task<List<ValidationException>> ValidateMessageToFrontAsync(MessageDto messageDto)
    {
        var errors = new List<ValidationException>();
        
        if (string.IsNullOrWhiteSpace(messageDto.Label))
        {
            errors.Add(new ValidationException("Сообщение не заполнено при отправке сообщения фронту."));
        }

        if (messageDto.Module is UserConnectionModuleEnum.Undefined or not UserConnectionModuleEnum.Communications)
        {
            errors.Add(new ValidationException("Недопустимый тип модуля при отправке сообщения фронту."));
        }

        if (messageDto.DialogId <= 0)
        {
            errors.Add(new ValidationException("Id диалога невалиден при отправке сообщения фронту."));
        }

        if (messageDto.UserCode == Guid.Empty)
        {
            errors.Add(new ValidationException("Недопустимый код пользователя при отправке сообщения фронту."));
        }
        
        if (messageDto.CreatedBy <= 0)
        {
            errors.Add(new ValidationException("Id пользователя невалиден при отправке сообщения фронту."));
        }

        return await Task.FromResult(errors);
    }

    private async Task ThrowChannelExceptionAsync()
    {
        var exChannel = new InvalidOperationException("Канал кролика был NULL.");
        await _discordService.SendNotificationErrorAsync(exChannel).ConfigureAwait(false);

        throw exChannel;
    }

    #endregion
}