using System.Runtime.CompilerServices;
using AutoMapper;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Messaging.Chat;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Chat;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Base.Builders;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Chat.Input;
using LeokaEstetica.Platform.Models.Dto.Chat.Output;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Redis.Abstractions.Client;
using LeokaEstetica.Platform.Redis.Abstractions.Connection;
using Microsoft.AspNetCore.SignalR;
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
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ProjectManagementHub> _logger;
    private readonly IConnectionService _connectionService;
    private readonly IDiscordService _discordService;
    private readonly IChatService _chatService;
    private readonly IClientConnectionService _clientConnectionService;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="chatRepository">Репозиторий чата.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="connectionService">Сервис подключений Redis.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="chatService">Сервис чатов.</param>
    /// <param name="clientConnectionService">Сервис подключений клиентов кэша.</param>
    public ProjectManagementHub(IChatRepository chatRepository,
        IMapper mapper,
        IUserRepository userRepository,
        ILogger<ProjectManagementHub> logger,
        IConnectionService connectionService,
        IDiscordService discordService,
        IChatService chatService,
        IClientConnectionService clientConnectionService)
    {
        _chatRepository = chatRepository;
        _mapper = mapper;
        _userRepository = userRepository;
        _logger = logger;
        _connectionService = connectionService;
        _discordService = discordService;
        _chatService = chatService;
        _clientConnectionService = clientConnectionService;
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
            
            var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);
            
            await Clients
                .Client(connectionId)
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
                Enum.Parse<DiscussionTypeEnum>(json!.DiscussionType), account, json.DiscussionTypeId,
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
    public async Task SendMessageAsync(string message, long dialogId, string account, string token)
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

            var result = await _chatService.SendMessageAsync(message, dialogId, userId, token, true, true);
            result.ActionType = DialogActionType.Message.ToString();

            var clients = await _clientConnectionService.CreateClientsResultAsync(dialogId, userId, token);
            
            await Clients
                .Clients(clients.AsList())
                .SendAsync("listenSendMessage", result)
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
    public Task GetProfileDialogsAsync(string account, string token)
    {
        throw new NotImplementedException("В модуле УП эта логика не предполагается.");
    }
}