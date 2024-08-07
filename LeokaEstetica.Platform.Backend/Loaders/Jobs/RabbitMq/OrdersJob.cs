﻿using System.Text;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Base.Models.IntegrationEvents.Orders;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Base.Commerce;
using LeokaEstetica.Platform.Processing.Abstractions.Commerce;
using LeokaEstetica.Platform.Processing.Enums;
using LeokaEstetica.Platform.Services.Abstractions.Subscription;
using Newtonsoft.Json;
using Quartz;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LeokaEstetica.Platform.Backend.Loaders.Jobs.RabbitMq;

/// <summary>
/// Класс джобы заказов.
/// </summary>
[DisallowConcurrentExecution]
internal sealed class OrdersJob : IJob
{
    private IModel? _channel;
    private readonly ICommerceRepository _commerceRepository;
    private readonly ILogger<OrdersJob> _logger;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IFareRuleRepository _fareRuleRepository;
    private readonly IGlobalConfigRepository _globalConfigRepository;
    private readonly IDiscordService _discordService;
    private readonly ICommerceService _commerceService;

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
    /// <param name="commerceRepository">Репозиторий коммерции.</param>
    /// <param name="logger">Сервис логов.</param>
    /// <param name="subscriptionService">Сервис подписок.</param>
    /// <param name="fareRuleRepository">Репозиторий тарифов.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфигов.</param>
    /// <param name="discordService">Сервис дискорд.</param>
    /// <param name="commerceService">Сервис коммерции.</param>
    public OrdersJob(ICommerceRepository commerceRepository, 
        ILogger<OrdersJob> logger, 
        ISubscriptionService subscriptionService, 
        IFareRuleRepository fareRuleRepository,
        IGlobalConfigRepository globalConfigRepository,
        IDiscordService discordService,
        ICommerceService commerceService)
    {
        _commerceRepository = commerceRepository;
        _logger = logger;
        _subscriptionService = subscriptionService;
        _fareRuleRepository = fareRuleRepository;
        _globalConfigRepository = globalConfigRepository;
        _discordService = discordService;
        _commerceService = commerceService;
    }
    
    /// <summary>
    /// Метод запускает логику фоновой задачи.
    /// </summary>
    /// <param name="context">Выполняемый контекст джобы.</param>
    public async Task Execute(IJobExecutionContext context)
    {
        var isEnabledJob = await _globalConfigRepository
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.JobsMode.ORDERS_JOB_MODE_ENABLED);

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
        
        var flags = QueueTypeEnum.OrdersQueue | QueueTypeEnum.OrdersQueue;

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
            await CheckOrderStatusAsync();
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Метод выполняет работу джобы.
    /// </summary>
    private async Task CheckOrderStatusAsync()
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            var logger = _logger; // Вне скоупа логгер не пишет логи, поэтому должны иметь внутренний логгер тут.
            logger.LogInformation("Начали обработку сообщения из очереди заказов...");

            // Если очередь не пуста, то будем парсить результат для проверки статуса заказов.
            if (!ea.Body.IsEmpty)
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var orderEvent = JsonConvert.DeserializeObject<OrderEvent>(message);

                if (orderEvent is null)
                {
                    throw new InvalidOperationException("Событие не содержит нужных данных." +
                                                        $" Получили сообщение из очереди заказов: {message}");
                }

                PaymentStatusEnum newOrderStatus;
                PaymentStatusEnum oldStatusSysName;

                try
                {
                    // Проверяем статус платежа в ПС.
                    newOrderStatus = await _commerceService.CheckOrderStatusAsync(orderEvent.PaymentId);
                
                    // Получаем старый статус платежа до проверки в ПС.
                    oldStatusSysName = PaymentStatus.GetPaymentStatusBySysName(orderEvent.StatusSysName);
                }
                
                catch (Exception ex)
                {
                    logger.LogCritical(ex, "Ошибка при чтении очереди заказов.");
                        
                    await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
                    throw;
                }

                // Если статус заказа изменился в ПС, то обновляем его статус в БД.
                if (newOrderStatus != oldStatusSysName)
                {
                    try
                    {
                        // Обновляем статус заказа в нашей БД.
                        var updatedOrderStatus = await _commerceRepository.UpdateOrderStatusAsync(
                            newOrderStatus.ToString(), newOrderStatus.GetEnumDescription(), orderEvent.PaymentId,
                            orderEvent.OrderId);

                        if (!updatedOrderStatus)
                        {
                            var ex = new InvalidOperationException("Не удалось найти заказ для обновления статуса. " +
                                                                   $"OrderId: {orderEvent.OrderId}." +
                                                                   $"PaymentId: {orderEvent.PaymentId}");
                            throw ex;
                        }

                        var publicId = orderEvent.PublicId;
                        var fareRule = await _fareRuleRepository.GetFareRuleByPublicIdAsync(publicId);

                        if (fareRule.FareRule is null)
                        {
                            throw new InvalidOperationException("Ошибка получения тарифа. " +
                                                                $"PublicId: {publicId}.");
                        }

                        // Для бесплатного тарифа нет срока подписки.
                        if (!fareRule.FareRule.IsFree)
                        {
                            // Проставляем подписку и даты подписки пользователю.
                            await _subscriptionService.SetUserSubscriptionAsync(orderEvent.UserId, publicId,
                                orderEvent.Month, orderEvent.OrderId, fareRule.FareRule.RuleId);
                        }
                        
                        // Если статус платежа в ПС ожидает подтверждения, то подтверждаем его, чтобы списать ДС.
                        // Подтвердить в ПС можно только заказы в статусе waiting_for_capture.
                        if (newOrderStatus == PaymentStatusEnum.WaitingForCapture)
                        {
                            await _commerceService.ConfirmPaymentAsync(orderEvent.PaymentId,
                                new Amount(orderEvent.Price, orderEvent.Currency));
                        }
                    }
                
                    catch (Exception ex)
                    {
                        logger.LogCritical(ex, "Ошибка при чтении очереди заказов.");
                        
                        await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
                        throw;
                    }

                    if (_channel is null)
                    {
                        throw new InvalidOperationException("Канал кролика был NULL.");
                    }
                    
                    // Подтверждаем сообщение, чтобы дропнуть его из очереди.
                    _channel.BasicAck(ea.DeliveryTag, false);
                }

                // Статус в ПС не изменился, оставляем его в очереди.
                else
                {
                    logger.LogInformation("Оставили сообщение в очереди заказов...");
                    
                    if (_channel is null)
                    {
                         throw new InvalidOperationException("Канал кролика был NULL.");
                    }
                    
                    _channel.BasicRecoverAsync(false);
                }
            }

            logger.LogInformation("Закончили обработку сообщения из очереди заказов...");

            await Task.Yield();
        };

        _channel.BasicConsume(_queueName, false, consumer);

        await Task.CompletedTask;
    }
}