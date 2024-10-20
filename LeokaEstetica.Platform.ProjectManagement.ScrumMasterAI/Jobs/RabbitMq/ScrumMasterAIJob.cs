﻿using System.Runtime.CompilerServices;
using System.Text;
using LeokaEstetica.Platform.Base.Abstractions.Messaging.Chat;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI.IntegrationEvents;
using LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI.Models;
using LeokaEstetica.Platform.ProjectManagment.Documents.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Newtonsoft.Json;
using Quartz;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.ProjectManagement")]

namespace LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI.Jobs.RabbitMq;

/// <summary>
/// Класс джобы нейросети Scrum Master AI.
/// </summary>
[DisallowConcurrentExecution]
internal sealed class ScrumMasterAIJob : IJob
{
    private IModel? _channelMessages;
    private IModel? _channelAnalysis;
    private readonly ILogger<ScrumMasterAIJob> _logger;
    private readonly IGlobalConfigRepository _globalConfigRepository;
    private readonly IDiscordService _discordService;
    private readonly IFileManagerService _fileManagerService;
    private readonly IScrumMasterAiRepository _scrumMasterAiRepository;
    private readonly IChatService _chatService;

    /// <summary>
    /// Название очереди для сообщений.
    /// </summary>
    private readonly string _messageQueueName = string.Empty;

    /// <summary>
    /// Счетчик кол-ва подключений во избежание дублей подключений.
    /// </summary>
    private static uint _counterMessageQueue;

    /// <summary>
    /// Название очереди для анализа.
    /// </summary>
    private readonly string _analysisQueueName = string.Empty;

    /// <summary>
    /// Счетчик кол-ва подключений во избежание дублей подключений.
    /// </summary>
    private static uint _counterAnalysisQueue;
    
    private readonly Lazy<IHubNotificationService> _hubNotificationService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Сервис логов.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфигов.</param>
    /// <param name="discordService">Сервис дискорд.</param>
    /// <param name="fileManagerService">Сервис работы с файлами.</param>
    /// <param name="scrumMasterAiRepository">Репозиторий нейросети Scrum Master AI.</param>
    /// <param name="chatService">Сервис чата.</param>
    /// <param name="hubNotificationService">Сервис уведомлений хабов.</param>
    public ScrumMasterAIJob(ILogger<ScrumMasterAIJob> logger,
        IGlobalConfigRepository globalConfigRepository,
        IDiscordService discordService,
        IFileManagerService fileManagerService,
        IScrumMasterAiRepository scrumMasterAiRepository,
        IChatService chatService,
        Lazy<IHubNotificationService> hubNotificationService)
    {
        _logger = logger;
        _globalConfigRepository = globalConfigRepository;
        _discordService = discordService;
        _fileManagerService = fileManagerService;
        _scrumMasterAiRepository = scrumMasterAiRepository;
        _chatService = chatService;
        _hubNotificationService = hubNotificationService;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод выполняет логику джобы.
    /// </summary>
    /// <param name="context">Контекст выполнения джобы.</param>
    public async Task Execute(IJobExecutionContext context)
    {
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

        // Если кол-во подключений уже больше 1, то не будем плодить их,
        // а в рамках одного подключения будем работать с очередью.
        if (_counterMessageQueue < 1)
        {
            var flags = QueueTypeEnum.ScrumMasterAiMessage | QueueTypeEnum.ScrumMasterAiMessage;

            try
            {
                var connection1 = connection.CreateConnection();
                _channelMessages = connection1.CreateModel();

                _channelMessages.QueueDeclare(
                    queue: _messageQueueName.CreateQueueDeclareNameFactory(dataMap.GetString("Environment")!, flags),
                    durable: false, exclusive: false, autoDelete: true, arguments: null);
            }

            catch (Exception ex)
            {
                await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);

                _logger.LogError(ex, ex.Message);
                throw;
            }

            _counterMessageQueue++;
        }

        // Если кол-во подключений уже больше 1, то не будем плодить их,
        // а в рамках одного подключения будем работать с очередью.
        if (_counterAnalysisQueue < 1)
        {
            var flags = QueueTypeEnum.ScrumMasterAiAnalysis | QueueTypeEnum.ScrumMasterAiAnalysis;

            try
            {
                var connection1 = connection.CreateConnection();
                _channelAnalysis = connection1.CreateModel();

                _channelAnalysis.QueueDeclare(
                    queue: _analysisQueueName.CreateQueueDeclareNameFactory(dataMap.GetString("Environment")!, flags),
                    durable: false, exclusive: false, autoDelete: true, arguments: null);
            }

            catch (Exception ex)
            {
                await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);

                _logger.LogError(ex, ex.Message);
                throw;
            }

            _counterAnalysisQueue++;
        }

        // Если канал не был создан, то не будем дергать память.
        if (_channelMessages is not null || _channelAnalysis is not null)
        {
            // Выполняем параллельные задачи нейросети из разных очередей.
            Parallel.Invoke(RunClassificationNetworkWorkMessagesAsync, RunClassificationNetworkWorkAnalysisAsync);
        }

        await Task.CompletedTask;
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод выполняет работу из очереди сообщений.
    /// </summary>
    private async void RunClassificationNetworkWorkMessagesAsync()
    {
        var isEnabledJob = await _globalConfigRepository
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.ArtificialIntelligenceConfig.SCRUM_MASTER_AI_MESSAGES);

        // Если режим нейросети выкл, то не нагружаем ЦПУ и память.
        if (!isEnabledJob)
        {
            return;
        }

        await ClassificationNetworkMessagesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Метод выполняет работу из очереди анализа.
    /// </summary>
    private async void RunClassificationNetworkWorkAnalysisAsync()
    {
        var isEnabledJob = await _globalConfigRepository
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.ArtificialIntelligenceConfig.SCRUM_MASTER_AI_ANALYSiS);

        // Если режим нейросети выкл, то не нагружаем ЦПУ и память.
        if (!isEnabledJob)
        {
            return;
        }

        await ClassificationNetworkAnalysisAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Метод выполняет классификацию нейросетью из очереди сообщений.
    /// </summary>
    private async Task ClassificationNetworkMessagesAsync()
    {
        try
        {
            var consumer = new AsyncEventingBasicConsumer(_channelMessages);
            consumer.Received += async (_, ea) =>
            {
                var logger = _logger;
                logger.LogInformation("Начали обработку сообщения из очереди сообщений для нейросети...");

                try
                {
                    // Если очередь не пуста, то будем парсить результат для проверки сообщений для нейросети.
                    if (!ea.Body.IsEmpty)
                    {
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        var @event = JsonConvert.DeserializeObject<MessageClassificationEvent>(message);

                        if (@event is null)
                        {
                            throw new InvalidOperationException(
                                "Событие не содержит нужных данных." +
                                $" Получили сообщение из очереди сообщений для нейросети: {message}");
                        }

                        if (string.IsNullOrWhiteSpace(@event.Message))
                        {
                            throw new InvalidOperationException(
                                "Не передано сообщение для анализа нейросетью. " +
                                $"Данные события: {JsonConvert.SerializeObject(@event)}");
                        }

                        // Странный кейс, не ломаем приложением, но логируем такое.
                        if (@event.ScrumMasterAiEventType == ScrumMasterAiEventTypeEnum.None)
                        {
                            var ex = new InvalidOperationException(
                                "Неизвестный тип события из очереди сообщений нейросети. " +
                                "Оставили сообщение в очереди сообщений для нейросети. " +
                                "Событие должно было быть обработано из очереди сообщений нейросети," +
                                " но осталось в очереди. " +
                                $"Требуется исправление этого. Данные события: {JsonConvert.SerializeObject(@event)}");

                            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);

                            logger.LogError(ex, ex.Message);

                            // TODO: Если бахнет кейс с null, то обработаем его тут.
                            _channelMessages.BasicRecoverAsync(false);
                        }

                        else if (@event.ScrumMasterAiEventType == ScrumMasterAiEventTypeEnum.Message)
                        {
                            // TODO: Добавить отображение уведомления фронту о том, что знаем и разбираемся в таком кейсе.
                            // Если токена не было - критичная ситуация, логируем такое, но не ломаем приложение.
                            if (string.IsNullOrWhiteSpace(@event.ConnectionId))
                            {
                                var ex = new InvalidOperationException(
                                    "Токен был невалиден (null или пустой) при парсинге из очереди сообщений нейросети." +
                                    $" Данные события: {JsonConvert.SerializeObject(@event)}");
                                _logger.LogError(ex, ex.Message);

                                await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);

                                return;
                            }

                            var version = await _scrumMasterAiRepository.GetLastNetworkVersionAsync();

                            if (string.IsNullOrWhiteSpace(version))
                            {
                                throw new InvalidOperationException(
                                    "Не удалось получить актуальную версию модели нейросети: scrum_master_ai_message");
                            }
                            
                            var trainedModelStream = await _fileManagerService.DownloadNetworkModelAsync(version,
                                "scrum_master_ai_message.zip");
                                
                            var mlContext = new MLContext();

                            // Загружаем нейросети ее опыт предыдущих эпох.
                            var loadЕrainedModel = mlContext.Model.Load(trainedModelStream, out var _);

                            // Нейросеть проводит прогнозирование.
                            var predEngine = mlContext.Model
                                .CreatePredictionEngine<MessageClassification, MessageClassificationPrediction>(
                                    loadЕrainedModel);

                            // Результат ответа нейросети после прогнозирования.
                            var prediction = predEngine.Predict(new MessageClassification
                            {
                                Message = @event.Message
                            });

                            // TODO: Добавить отображение уведомления фронту о том, что знаем и разбираемся в таком кейсе.
                            // Если нейросеть не дала ответа - критичная ситуация, логируем такое, но не ломаем приложение.
                            if (string.IsNullOrWhiteSpace(prediction.Message))
                            {
                                var ex = new InvalidOperationException(
                                    "От нейросети не было получено ответа. " +
                                    "Требуется дообучение нейросети (она глупая либо не правильно провела классификацию.)." +
                                    $" Данные события: {JsonConvert.SerializeObject(@event)}. " +
                                    $"Данные ответа нейросети: {JsonConvert.SerializeObject(prediction)}.");
                                _logger.LogError(ex, ex.Message);

                                await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);

                                return;
                            }

                            if (@event.DialogId <= 0)
                            {
                                throw new InvalidOperationException(
                                    "Обнаружен невалидный Id диалога. Нейросеть не знает в какой диалог отвечать. " +
                                    $"Данные ответа нейросети: {JsonConvert.SerializeObject(prediction)}. " +
                                    $"Нейросеть хотела ответить: {prediction.Message}.");
                            }
                            
                            // Пишем ответ нейросети в БД.
                            // - 1 это Id нейросети. Пока хардкодим, если нейросетей станет несколько,
                            // то будем получать из БД.
                            await _chatService.SendMessageAsync(prediction.Message, @event.DialogId, -1,
                                @event.ConnectionId, true, true).ConfigureAwait(false);

                            // Отправляем результат классификации ответа нейросети на фронт.
                            await _hubNotificationService.Value
                                .SendClassificationNetworkMessageResultAsync(prediction.Message, @event.ConnectionId,
                                    @event.DialogId)
                                .ConfigureAwait(false);

                            // TODO: Если бахнет кейс с null, то обработаем его тут.
                            // Подтверждаем сообщение, чтобы дропнуть его из очереди.
                            _channelMessages.BasicAck(ea.DeliveryTag, false);
                        }

                        // Недопустимая ситуация, но не ломаем приложением, а просто логируем кейс.
                        else
                        {
                            var ex = new InvalidOperationException(
                                "Оставили сообщение в очереди сообщений для нейросети. " +
                                "Событие должно было быть обработано из очереди сообщений нейросети," +
                                " но осталось в очереди. " +
                                $"Требуется исправление этого. Данные события: {JsonConvert.SerializeObject(@event)}");
                            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);

                            logger.LogError(ex, ex.Message);

                            // TODO: Если бахнет кейс с null, то обработаем его тут.
                            _channelMessages.BasicRecoverAsync(false);
                        }
                    }

                    logger.LogInformation("Закончили обработку сообщения из очереди сообщений для нейросети...");

                    await Task.Yield();
                }

                catch (Exception ex)
                {
                    await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);

                    logger.LogError(ex, ex.Message);
                    throw;
                }
            };

            _channelMessages.BasicConsume(_messageQueueName, false, consumer);

            await Task.CompletedTask;
        }

        catch (Exception ex)
        {
            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);

            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод выполняет классификацию нейросетью из очереди анализа.
    /// </summary>
    private async Task ClassificationNetworkAnalysisAsync()
    {
        try
        {
            await Task.CompletedTask;
        }

        catch (Exception ex)
        {
            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);

            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion
}