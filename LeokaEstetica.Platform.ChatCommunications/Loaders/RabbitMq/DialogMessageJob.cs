using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using Quartz;
using RabbitMQ.Client;

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
    public DialogMessageJob(ILogger<DialogMessageJob> logger,
        IGlobalConfigRepository globalConfigRepository,
        IDiscordService discordService)
    {
        _logger = logger;
        _globalConfigRepository = globalConfigRepository;
        _discordService = discordService;
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
            
        }

        await Task.CompletedTask;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}