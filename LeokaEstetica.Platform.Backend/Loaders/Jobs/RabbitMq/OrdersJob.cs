using System.Text;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Base.Models.IntegrationEvents.Orders;
using LeokaEstetica.Platform.CallCenter.Abstractions.Vacancy;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Messaging.Abstractions.Mail;
using LeokaEstetica.Platform.Models.Dto.Base.Commerce;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Processing.Abstractions.Commerce;
using LeokaEstetica.Platform.Processing.BuilderData;
using LeokaEstetica.Platform.Processing.Builders.Order;
using LeokaEstetica.Platform.Processing.Enums;
using LeokaEstetica.Platform.Services.Abstractions.Subscription;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IVacancyModerationService _vacancyModerationService;
    private readonly IUserRepository _userRepository;
    private readonly IMailingsService _mailingsService;
    private readonly IVacancyRepository _vacancyRepository;

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
    /// <param name="subscriptionRepository">Репозиторий подписок пользователей.</param>
    /// <param name="vacancyRepository">Репозиторий вакансий.</param>
    /// <param name="projectRepository">Репозиторий проектов.</param>
    /// <param name="vacancyModerationService">Сервис модерации вакансий.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="mailingsService">Сервис рассылок.</param>
    public OrdersJob(ICommerceRepository commerceRepository,
        ILogger<OrdersJob> logger,
        ISubscriptionService subscriptionService,
        IFareRuleRepository fareRuleRepository,
        IGlobalConfigRepository globalConfigRepository,
        IDiscordService discordService,
        ICommerceService commerceService,
        ISubscriptionRepository subscriptionRepository,
        IVacancyRepository vacancyRepository,
        IProjectRepository projectRepository,
        IVacancyModerationService vacancyModerationService,
        IUserRepository userRepository,
        IMailingsService mailingsService)
    {
        _commerceRepository = commerceRepository;
        _logger = logger;
        _subscriptionService = subscriptionService;
        _fareRuleRepository = fareRuleRepository;
        _globalConfigRepository = globalConfigRepository;
        _discordService = discordService;
        _commerceService = commerceService;
        _subscriptionRepository = subscriptionRepository;
        _vacancyRepository = vacancyRepository;
        _projectRepository = projectRepository;
        _vacancyModerationService = vacancyModerationService;
        _userRepository = userRepository;
        _mailingsService = mailingsService;
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
                    BaseOrderBuilder? orderBuilder = null;

                    if (orderEvent.OrderType == OrderTypeEnum.Undefined)
                    {
                        throw new InvalidOperationException("Неизвестный тип заказа. " +
                                                            $"OrderType: {orderEvent.OrderType}.");
                    }

                    // Проверяем статус платежа в ПС.
                    if (orderEvent.OrderType == OrderTypeEnum.FareRule)
                    {
                        BaseOrderBuilder builder = new FareRuleOrderBuilder(_subscriptionRepository,
                            _commerceRepository);
                        orderBuilder = (FareRuleOrderBuilder)builder;
                    }
                    
                    else if (orderEvent.OrderType == OrderTypeEnum.CreateVacancy)
                    {
                        BaseOrderBuilder builder = new PostVacancyOrderBuilder(_subscriptionRepository,
                            _commerceRepository);
                        orderBuilder = (PostVacancyOrderBuilder)builder;
                    }

                    if (orderBuilder is null)
                    {
                        throw new InvalidOperationException("Ошибка определения типа билдера. " +
                                                            $"OrderType: {orderEvent.OrderType}.");
                    }
                    
                    orderBuilder.OrderData ??= new OrderData();
                    orderBuilder.OrderData.PaymentId = orderEvent.PaymentId;
                    newOrderStatus = await _commerceService.CheckOrderStatusAsync(orderBuilder);

                    if (string.IsNullOrWhiteSpace(orderEvent.StatusSysName))
                    {
                        throw new InvalidOperationException("Ошибка определения системного названия статуса. " +
                                                            $"StatusSysName: {orderEvent.StatusSysName}. " +
                                                            $"OrderEvent: {orderEvent}.");
                    }

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
                        if (string.IsNullOrWhiteSpace(orderEvent.PaymentId))
                        {
                            var ex = new InvalidOperationException("Ошибка определения Id платежа в ПС. " +
                                                                   $"PaymentId: {orderEvent.PaymentId}. " +
                                                                   $"OrderEvent: {orderEvent}.");
                            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
                            throw ex;
                        }
                        
                        // Обновляем статус заказа в нашей БД.
                        var updatedOrderStatus = await _commerceRepository.UpdateOrderStatusAsync(
                            newOrderStatus.ToString(), newOrderStatus.GetEnumDescription(), orderEvent.PaymentId,
                            orderEvent.OrderId);

                        if (!updatedOrderStatus)
                        {
                            var ex = new InvalidOperationException("Не удалось найти заказ для обновления статуса. " +
                                                                   $"OrderId: {orderEvent.OrderId}." +
                                                                   $"PaymentId: {orderEvent.PaymentId}");
                            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
                            throw ex;
                        }

                        var publicId = orderEvent.PublicId;
                        var fareRule = await _fareRuleRepository.GetFareRuleByPublicIdAsync(publicId);

                        // Для бесплатного тарифа нет срока подписки.
                        // Кол-во месяцев есть только у оплаты заказа на тариф.
                        if (!fareRule.FareRule!.IsFree && orderEvent.PaymentMonth.HasValue)
                        {
                            // Проставляем подписку и даты подписки пользователю.
                            await _subscriptionService.SetUserSubscriptionAsync(orderEvent.CreatedBy, publicId,
                                orderEvent.PaymentMonth, orderEvent.OrderId, fareRule.FareRule.RuleId);
                        }
                        
                        // Если статус платежа в ПС ожидает подтверждения, то подтверждаем его, чтобы списать ДС.
                        // Подтвердить в ПС можно только заказы в статусе waiting_for_capture.
                        if (newOrderStatus == PaymentStatusEnum.WaitingForCapture)
                        {
                            BaseOrderBuilder? orderBuilder = null;

                            // Проверяем статус платежа в ПС.
                            if (orderEvent.OrderType == OrderTypeEnum.FareRule)
                            {
                                BaseOrderBuilder builder = new FareRuleOrderBuilder(_subscriptionRepository,
                                    _commerceRepository);
                                orderBuilder = (FareRuleOrderBuilder)builder;
                            }
                    
                            else if (orderEvent.OrderType == OrderTypeEnum.CreateVacancy)
                            {
                                
                                BaseOrderBuilder builder = new PostVacancyOrderBuilder(_subscriptionRepository,
                                    _commerceRepository);
                                orderBuilder = (PostVacancyOrderBuilder)builder;
                            }

                            if (orderBuilder is null)
                            {
                                var ex = new InvalidOperationException("Ошибка определения типа билдера. " +
                                                                       $"OrderType: {orderEvent.OrderType}.");
                                await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
                                throw ex;
                            }

                            if (orderBuilder.OrderData is null)
                            {
                                var ex = new InvalidOperationException("Ошибка подготовительных данных билдера. " +
                                                                       $"OrderType: {orderEvent.OrderType}.");
                                await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
                                throw ex;
                            }

                            orderBuilder.OrderData.Amount ??= new Amount(orderEvent.Price,
                                orderEvent.Currency.ToString());
                            orderBuilder.OrderData.PaymentId = orderEvent.PaymentId;

                            await _commerceService.ConfirmPaymentAsync(orderBuilder);
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
                        var ex = new InvalidOperationException("Канал кролика был NULL.");
                        await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
                        throw ex;
                    }

                    if (orderEvent.OrderType == OrderTypeEnum.CreateVacancy
                        && newOrderStatus != PaymentStatusEnum.Succeeded)
                    {
                        logger.LogInformation("Оставили сообщение в очереди заказов...");
                    
                        if (_channel is null)
                        {
                            var ex = new InvalidOperationException("Канал кролика был NULL.");
                            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
                            throw ex;
                        }
                    
                        _channel.BasicRecoverAsync(false);

                        await Task.Yield();
                    }

                    // Если статус подтвержден, и тип заказа создание вакансии,
                    // то создаем вакансию пользователя и отправляем ее на модерацию.
                    else if (orderEvent.OrderType == OrderTypeEnum.CreateVacancy
                             && newOrderStatus == PaymentStatusEnum.Succeeded)
                    {
                        var vacancyJson = JToken.Parse(message)["VacancyOrderData"];

                        if (vacancyJson is null)
                        {
                            var ex = new InvalidOperationException("Ошибка парсинга данных вакансии. " +
                                                                   $"OrderEvent: {message}.");
                            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
                            throw ex;
                        }
                    
                        var vacancyString = JsonConvert.SerializeObject(vacancyJson);
                        var vacancy = JsonConvert.DeserializeObject<VacancyInput>(vacancyString);
                        
                        if (vacancy is null)
                        {
                            var ex = new InvalidOperationException(
                                "Данные вакансии не были заполнены для создания вакансии.");
                            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
                            throw ex;
                        }

                        try
                        {
                            // Добавляем вакансию в таблицу вакансий пользователя.
                            var createdVacancy = await _vacancyRepository.CreateVacancyAsync(vacancy,
                                orderEvent.CreatedBy);
                            var vacancyId = createdVacancy.VacancyId;

                            if (vacancy.ProjectId <= 0)
                            {
                                var ex = new InvalidOperationException(
                                    "ProjectId не был заполнен. " +
                                    "Вакансия не может быть создана без привязки к проекту.");
                                await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
                                throw ex;
                            }
                        
                            // Привязываем вакансию к проекту.
                            await _projectRepository.AttachProjectVacancyAsync(vacancy.ProjectId, vacancyId);
                        
                            // Отправляем вакансию на модерацию.
                            await _vacancyModerationService.AddVacancyModerationAsync(vacancyId);
                        
                            // TODO: Из джобы так не сделать. Нужно другое решение как уведомлять пользователя. 
                            // TODO: Это будет делать джоба MailSenderJob.
                            // Отправляем уведомление об успешном создании вакансии и отправки ее на модерацию.
                            // await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                            //     "Данные успешно сохранены. Вакансия отправлена на модерацию.",
                            //     NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotificationSuccessCreatedUserVacancy",
                            //     userCode, UserConnectionModuleEnum.Main);

                            var user = await _userRepository.GetUserPhoneEmailByUserIdAsync(orderEvent.CreatedBy);
                        
                            // Отправляем уведомление о созданной вакансии владельцу.
                            await _mailingsService.SendNotificationCreateVacancyAsync(user.Email,
                                createdVacancy.VacancyName, vacancyId);
                        
                            // Отправляем уведомление о созданной вакансии в дискорд.
                            await _discordService.SendNotificationCreatedVacancyBeforeModerationAsync(vacancyId);
                            
                            // Проставляем признак оплаты вакансии.
                            await _vacancyRepository.SetVacancyPaymentAsync(vacancyId, true);
                        }
                        
                        catch (Exception ex)
                        {
                            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
                            throw;
                        }
                        
                        // Подтверждаем сообщение, чтобы дропнуть его из очереди.
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }

                    if (orderEvent.OrderType != OrderTypeEnum.CreateVacancy)
                    {
                        // Подтверждаем сообщение, чтобы дропнуть его из очереди.
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                }

                // Статус в ПС не изменился, оставляем его в очереди.
                else
                {
                    logger.LogInformation("Оставили сообщение в очереди заказов...");
                    
                    if (_channel is null)
                    {
                         var ex = new InvalidOperationException("Канал кролика был NULL.");
                         await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
                         throw ex;
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