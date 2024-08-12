using System.Runtime.CompilerServices;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Messaging.Chat;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Chat;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Base.Builders;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Extensions;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Base.Factors;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Chat.Input;
using LeokaEstetica.Platform.Models.Dto.Chat.Output;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.RabbitMq.Abstractions;
using LeokaEstetica.Platform.Redis.Abstractions.Client;
using LeokaEstetica.Platform.Redis.Abstractions.Connection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Enum = System.Enum;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.ProjectManagement")]

namespace LeokaEstetica.Platform.Notifications.Data;

/// <summary>
/// Класс хаба модуля УП (управление проектами).
/// </summary>
internal sealed class ProjectManagementHub : Hub, IHubService
{
    private readonly IChatRepository _chatRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ProjectManagementHub> _logger;
    private readonly IConnectionService _connectionService;
    private readonly IDiscordService _discordService;
    private readonly IChatService _chatService;
    private readonly IClientConnectionService _clientConnectionService;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="chatRepository">Репозиторий чата.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="connectionService">Сервис подключений Redis.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="chatService">Сервис чатов.</param>
    /// <param name="clientConnectionService">Сервис подключений клиентов кэша.</param>
    /// <param name="clientConnectionService">Сервис брокера сообщений.</param>
    /// <param name="httpClientFactory">Факторка Http-клиентов.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    public ProjectManagementHub(IChatRepository chatRepository,
        IUserRepository userRepository,
        ILogger<ProjectManagementHub> logger,
        IConnectionService connectionService,
        IDiscordService discordService,
        IChatService chatService,
        IClientConnectionService clientConnectionService,
        IRabbitMqService rabbitMqService,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _chatRepository = chatRepository;
        _userRepository = userRepository;
        _logger = logger;
        _connectionService = connectionService;
        _discordService = discordService;
        _chatService = chatService;
        _clientConnectionService = clientConnectionService;
        _rabbitMqService = rabbitMqService;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }
    
    public override Task OnConnectedAsync()
    {
        var userCode = Context.GetHttpContext().Request.Query["userCode"].ToString();
        var module = Enum.Parse<UserConnectionModuleEnum>(Context.GetHttpContext().Request.Query["module"].ToString());

        if (!string.IsNullOrEmpty(userCode))
        {
            _connectionService.AddConnectionIdCacheAsync(userCode, Context.ConnectionId, module)
                .ConfigureAwait(false);
        }
        
        return base.OnConnectedAsync();
    }
    
    public override Task OnDisconnectedAsync(Exception exception)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task GetDialogsAsync(string account, string token, long? objectId = null)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }

            var result = new ScrumMasterAiNetworkDialogResult
            {
                Dialogs = new List<ScrumMasterAiNetworkDialogOutput>(),
                ActionType = DialogActionType.All.ToString()
            };

            var dialogs = await _chatRepository.GetDialogsScrumMasterAiAsync(userId, objectId);

            result.Dialogs = await CreateDialogMessagesBuilder.CreateDialogAsync(dialogs,
                _chatRepository, _userRepository, userId, account);
            
            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);
            var key = userCode + "_" + UserConnectionModuleEnum.Main;
            var connection = await _connectionService.GetConnectionIdCacheAsync(key);
            
            await Clients
                .Client(connection.ConnectionId)
                .SendAsync("listenGetDialogs", result)
                .ConfigureAwait(false);
        }

        catch (Exception ex)
        {
            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
            
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task GetDialogAsync(string account, string token, string dialogInput)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }

            var json = JsonConvert.DeserializeObject<DialogInput>(dialogInput);

            if (json is null)
            {
                throw new InvalidOperationException("Не удалось распарсить входную модель диалога.");
            }

            var result = await _chatService.GetDialogAsync(json.DialogId,
                Enum.Parse<DiscussionTypeEnum>(json.DiscussionType), account, json.DiscussionTypeId,
                json.isManualNewDialog, token);
            
            result.ActionType = DialogActionType.Concrete.ToString();

            var clients = await _clientConnectionService.CreateClientsResultAsync(json.DialogId, userId, token);

            await Clients
                .Clients(clients.AsList())
                .SendAsync("listenGetDialog", result)
                .ConfigureAwait(false);
        }

        catch (Exception ex)
        {
            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
            
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SendMessageAsync(string message, long dialogId, string account, string token, string apiUrl)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            if (dialogId == 0)
            {
                throw new InvalidOperationException($"Id диалога не может быть пустым. Account: {account}");
            }

            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }

            using var httpClient = _httpClientFactory.CreateClient();
            httpClient.SetHttpClientRequestAuthorizationHeader(token);

            var queueType = string.Empty.CreateQueueDeclareNameFactory(_configuration["Environment"],
                QueueTypeEnum.ScrumMasterAiMessage);
                
            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);
            
            var key = userCode + "_" + UserConnectionModuleEnum.Main;
            var connection = await _connectionService.GetConnectionIdCacheAsync(key);

            // TODO: - 1 это Id нейросети. Пока хардкодим, если нейросетей станет несколько,
            // TODO: то будем получать из БД.
            var scrumMasterAiMessageEvent = ScrumMasterAiMessageEventFactory.CreateScrumMasterAiMessageEvent(message,
                connection.ConnectionId, -1, ScrumMasterAiEventTypeEnum.Message, dialogId);
            
            // Отправляем событие в кролика для ответа нейросети в джобе.
            await _rabbitMqService.PublishAsync(scrumMasterAiMessageEvent, queueType, _configuration);

            // Пишем сообщение в БД.
            await _chatService.SendMessageAsync(message, dialogId, userId, token, true, true).ConfigureAwait(false);
        }

        catch (Exception ex)
        {
            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
            
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public Task GetProfileDialogsAsync(string account, string token)
    {
        throw new NotImplementedException("В модуле УП эта логика не предполагается.");
    }
}