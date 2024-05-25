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

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Backend")]

namespace LeokaEstetica.Platform.Notifications.Data;

/// <summary>
/// Класс логики хаба для чатов.
/// Также используется для уведомлений у основного модуля системы.
/// </summary>
internal sealed class ChatHub : Hub, IHubService
{
    private readonly IChatRepository _chatRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ChatHub> _logger;
    private readonly IConnectionService _connectionService;
    private readonly IChatService _chatService;
    private readonly IDiscordService _discordService;
    private readonly IClientConnectionService _clientConnectionService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="chatRepository">Репозиторий чата.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="connectionService">Сервис подключений Redis.</param>
    /// <param name="chatService">Сервис чата.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="clientConnectionService">Сервис подключений клиентов кэша.</param>
    public ChatHub(IChatRepository chatRepository,
        IMapper mapper,
        IUserRepository userRepository,
        ILogger<ChatHub> logger,
        IConnectionService connectionService,
        IChatService chatService,
        IDiscordService discordService,
        IClientConnectionService clientConnectionService)
    {
        _chatRepository = chatRepository;
        _mapper = mapper;
        _userRepository = userRepository;
        _logger = logger;
        _connectionService = connectionService;
        _chatService = chatService;
        _discordService = discordService;
        _clientConnectionService = clientConnectionService;
    }

    #region Публичные методы

    /// <inheritdoc />
    public async Task GetDialogsAsync(string account, string token, long? projectId = null)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }

            var result = new ProjectDialogResult
            {
                Dialogs = new List<DialogOutput>(),
                ActionType = DialogActionType.All.ToString()
            };

            var dialogs = await _chatRepository.GetDialogsAsync(userId, projectId);
            var mapDialogs = _mapper.Map<List<ProfileDialogOutput>>(dialogs);

            result.Dialogs = await CreateDialogMessagesBuilder.CreateDialogAsync((dialogs, mapDialogs),
                _chatRepository, _userRepository, userId, _mapper, account);

            var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

            await Clients
                .Client(connectionId)
                .SendAsync("listenGetProjectDialogs", result)
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
            var json = JsonConvert.DeserializeObject<DialogInput>(dialogInput);

            if (json is null)
            {
                throw new InvalidOperationException("Не удалось распарсить входную модель диалога.");
            }

            // Просто логируем такой кейс, его не должно быть в этой логике.
            if (json.isManualNewDialog)
            {
                var ex = new InvalidOperationException("Передали признак принудительного создания диалога. " +
                                                       "В этом кейсе (обычные чаты) этого не должно было быть. " +
                                                       $"Входные данные: {dialogInput}");
                await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
            
                _logger.LogInformation(ex, ex.Message);
            }
            
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }

            // Тут isManualNewDialog всегда false, так как тут обычные чаты.
            var result = await _chatService.GetDialogAsync(json.DialogId,
                Enum.Parse<DiscussionTypeEnum>(json!.DiscussionType), account, json.DiscussionTypeId, false);
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

            var result = await _chatService.SendMessageAsync(message, dialogId, userId, token, true);
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
    public async Task GetProfileDialogsAsync(string account, string token)
    {
        try
        {
            var result = new ProfileDialogResult
            {
                ActionType = DialogActionType.All.ToString(),
                Dialogs = await _chatService.GetProfileDialogsAsync(account)
            };

            var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

            await Clients
                .Client(connectionId)
                .SendAsync("listenProfileDialogs", result)
                .ConfigureAwait(false);
        }

        catch (Exception ex)
        {
            await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
            
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    #endregion
}