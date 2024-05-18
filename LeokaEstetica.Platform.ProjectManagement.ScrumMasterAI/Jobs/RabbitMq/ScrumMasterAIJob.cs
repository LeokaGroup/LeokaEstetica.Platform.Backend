﻿using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI.Enums;
using LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI.IntegrationEvents;
using LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Newtonsoft.Json;
using Quartz;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Renci.SshNet;
using Renci.SshNet.Common;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.ProjectManagement")]

namespace LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI.Jobs.RabbitMq;

/// <summary>
/// Класс джобы нейросети Scrum Master AI.
/// </summary>
[DisallowConcurrentExecution]
internal sealed class ScrumMasterAIJob : IJob
{
    private readonly IModel? _channelMessages;
    private readonly IModel? _channelAnalysis;
    private readonly ILogger<ScrumMasterAIJob> _logger;
    private readonly IGlobalConfigRepository _globalConfigRepository;
    private readonly IDiscordService _discordService;
    private readonly Lazy<IProjectManagementNotificationService> _projectManagementNotificationService;

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

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="configuration">Зависимость конфигурации приложения.</param>
    /// <param name="logger">Сервис логов.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфигов.</param>
    /// <param name="discordService">Сервис дискорд.</param>
    /// <param name="projectManagementNotificationService">Сервис уведомлений модуля УП.</param>
    public ScrumMasterAIJob(IConfiguration configuration,
        ILogger<ScrumMasterAIJob> logger,
        IGlobalConfigRepository globalConfigRepository,
        IDiscordService discordService,
         Lazy<IProjectManagementNotificationService> projectManagementNotificationService)
    {
        _logger = logger;
        _globalConfigRepository = globalConfigRepository;
        _discordService = discordService;
        _projectManagementNotificationService = projectManagementNotificationService;

        var connection = new ConnectionFactory
        {
            HostName = configuration["RabbitMq:HostName"],
            Password = configuration["RabbitMq:Password"],
            UserName = configuration["RabbitMq:UserName"],
            DispatchConsumersAsync = true,
            Port = AmqpTcpEndpoint.UseDefaultPort,
            VirtualHost = configuration["RabbitMq:VirtualHost"],
            ContinuationTimeout = new TimeSpan(0, 0, 10, 0)
        };

        // Если кол-во подключений уже больше 1, то не будем плодить их,
        // а в рамках одного подключения будем работать с очередью.
        if (_counterMessageQueue < 1)
        {
            var connection1 = connection.CreateConnection();
            _channelMessages = connection1.CreateModel();

            _channelMessages.QueueDeclare(
                queue: _messageQueueName.CreateQueueDeclareNameFactory(configuration,
                    QueueTypeEnum.ScrumMasterAiMessage),
                durable: false, exclusive: false, autoDelete: true, arguments: null);

            _counterMessageQueue++;
        }

        // Если кол-во подключений уже больше 1, то не будем плодить их,
        // а в рамках одного подключения будем работать с очередью.
        if (_counterAnalysisQueue < 1)
        {
            var connection1 = connection.CreateConnection();
            _channelAnalysis = connection1.CreateModel();

            _channelAnalysis.QueueDeclare(
                queue: _analysisQueueName.CreateQueueDeclareNameFactory(configuration,
                    QueueTypeEnum.ScrumMasterAiAnalysis),
                durable: false, exclusive: false, autoDelete: true, arguments: null);

            _counterAnalysisQueue++;
        }
    }

    #region Публичные методы.

    /// <summary>
    /// Метод выполняет логику джобы.
    /// </summary>
    /// <param name="context">Контекст выполнения джобы.</param>
    public async Task Execute(IJobExecutionContext context)
    {
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
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.JobsMode.SCRUM_MASTER_AI_MESSAGES);

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
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.JobsMode.SCRUM_MASTER_AI_ANALYSiS);

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

                    // Странный кейс, не ломаем приложением, но логируем такое.
                    if (@event.ScrumMasterAiEventType == ScrumMasterAiEventTypeEnum.None)
                    {
                        var ex = new InvalidOperationException(
                            "Неизвестный тип события из очереди сообщений нейросети. " +
                            "Оставили сообщение в очереди сообщений для нейросети. " +
                            "Событие должно было быть обработано из очереди сообщений нейросети," +
                            " но осталось в очереди. " +
                            $"Требуется исправление этого. Данные события: {JsonConvert.SerializeObject(@event)}");
                        await _discordService.SendNotificationErrorAsync(ex);

                        logger.LogError(ex, ex.Message);

                        // TODO: Если бахнет кейс с null, то обработаем его тут.
                        _channelMessages.BasicRecoverAsync(false);
                    }
                    
                    else if (@event.ScrumMasterAiEventType == ScrumMasterAiEventTypeEnum.Message)
                    {
                        // Создаем контекст нейросети.
                        var mlContext = new MLContext(seed: 0);

                        var settings = await _globalConfigRepository.GetFileManagerSettingsAsync();

                        using var sftpClient = new SftpClient(settings.Host, settings.Port, settings.Login,
                            settings.Password);
                            
                        using var stream = new MemoryStream();

                        try
                        {
                            sftpClient.Connect();

                            if (!sftpClient.IsConnected)
                            {
                                throw new InvalidOperationException(
                                    "Sftp клиент не подключен. " +
                                    "Невозможно скачать опыт предыдущих эпох нейросети с сервера.");
                            }

                            // Скачиваем обученную модель с сервера, чтобы нейросеть получила весь свой опыт предыдущих эпох.
                            sftpClient.DownloadFile(settings.SftpTaskPath, stream);
                        }
                        
                        catch (Exception ex) when (ex is SshConnectionException or SocketException or ProxyException)
                        {
                            _logger.LogError(ex, "Ошибка подключения к серверу по Sftp.");
                            throw;
                        }

                        catch (SshAuthenticationException ex)
                        {
                            _logger.LogError(ex, "Ошибка аутентификации к серверу по Sftp.");
                            throw;
                        }

                        catch (SftpPermissionDeniedException ex)
                        {
                            _logger.LogError(ex, "Ошибка доступа к серверу по Sftp.");
                            throw;
                        }

                        catch (SshException ex)
                        {
                            _logger.LogError(ex, "Ошибка Sftp.");
                            throw;
                        }

                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Ошибка при скачивании опыта предыдущих эпох нейросети с сервера.");
                            throw;
                        }

                        finally
                        {
                            sftpClient.Disconnect();
                        }
            
                        // Сбрасываем позицию на 0, иначе у файла будет размер 0 байтов,
                        // если не сбросить указатель позиции в начало.
                        stream.Seek(0, SeekOrigin.Begin);

                        // Загружаем нейросети ее опыт предыдущих эпох.
                        var trainedModel = mlContext.Model.Load(stream, out var _);
                        
                        // TODO: Microsoft советует не использовать AppendCacheCheckpoint на больших данных!
                        // TODO: На малых и средних разрешается. Ускоряет при повторных итерациях обучения.
                        // TODO: P.S: Цитата Microsoft: Удалите AppendCacheCheckpoint при обработке очень больших наборов данных.
                        // var pipeline = mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Message",
                        //         outputColumnName: "Label")
                        //     .Append(mlContext.Transforms.Concatenate("Features"))
                        //     .AppendCacheCheckpoint(mlContext);
                        
                        // По дефолту у SdcaMaximumEntropy значения "Label", "Features".
                        // var trainingPipeline = pipeline
                        //     .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
                        //     .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

                        // Нейросеть проводит прогнозирование.
                        var predEngine = mlContext.Model
                            .CreatePredictionEngine<MessageClassification, MessageClassificationPrediction>(
                                trainedModel);
                            
                        // TODO: Если будет много полей, то вынести в приватный метод факторки.
                        // Результат ответа нейросети после прогнозирования.
                        var prediction = predEngine.Predict(new MessageClassification
                        {
                            Message = @event.Message
                        });
                        
                        // Обучаем модель.
                        // trainedModel = trainingPipeline.Fit(trainingDataView);

                        // TODO: Добавить отображение уведомления фронту о том, что знаем и разбираемся в таком кейсе.
                        // Если токена не было - критичная ситуация, логируем такое, но не ломаем приложение.
                        if (string.IsNullOrWhiteSpace(@event.Token))
                        {
                            var ex = new InvalidOperationException(
                                "Токен был невалиден (null или пустой) при парсинге из очереди сообщений нейросети." +
                                $" Данные события: {JsonConvert.SerializeObject(@event)}");
                            _logger.LogError(ex, ex.Message);

                            await _discordService.SendNotificationErrorAsync(ex);

                            return;
                        }
                        
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

                            await _discordService.SendNotificationErrorAsync(ex);

                            return;
                        }
                        
                        // Отправляем результат классификации ответа нейросети на фронт.
                        await _projectManagementNotificationService.Value
                            .SendClassificationNetworkMessageResultAsync(prediction.Message, @event.Token);

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
                        await _discordService.SendNotificationErrorAsync(ex);

                        logger.LogError(ex, ex.Message);

                        // TODO: Если бахнет кейс с null, то обработаем его тут.
                        _channelMessages.BasicRecoverAsync(false);
                    }
                }

                logger.LogInformation("Закончили обработку сообщения из очереди сообщений для нейросети...");

                await Task.Yield();
            };

            _channelMessages.BasicConsume(_messageQueueName, false, consumer);

            await Task.CompletedTask;
        }

        catch (Exception ex)
        {
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
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion
}