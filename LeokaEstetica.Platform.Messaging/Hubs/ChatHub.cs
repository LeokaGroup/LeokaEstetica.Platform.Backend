using System.Runtime.CompilerServices;
using AutoMapper;
using FluentValidation.Results;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Database.Chat;
using LeokaEstetica.Platform.Messaging.Abstractions.Chat;
using LeokaEstetica.Platform.Messaging.Builders;
using LeokaEstetica.Platform.Messaging.Validators;
using LeokaEstetica.Platform.Models.Dto.Chat.Input;
using LeokaEstetica.Platform.Models.Dto.Chat.Output;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Redis.Abstractions.Connection;
using LeokaEstetica.Platform.Redis.Models.Chat;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Backend")]

namespace LeokaEstetica.Platform.Messaging.Hubs;

/// <summary>
/// Класс логики хаба для чатов.
/// </summary>
internal sealed class ChatHub : Hub
{
    private readonly IChatRepository _chatRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ChatHub> _logger;
    private readonly IConnectionService _connectionService;
    private readonly IChatService _chatService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="chatRepository">Репозиторий чата.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="connectionService">Сервис подключений Redis.</param>
    /// <param name="chatService">Сервис чата.</param>
    public ChatHub(IChatRepository chatRepository,
        IMapper mapper,
        IUserRepository userRepository,
        ILogger<ChatHub> logger,
        IConnectionService connectionService,
        IChatService chatService)
    {
        _chatRepository = chatRepository;
        _mapper = mapper;
        _userRepository = userRepository;
        _logger = logger;
        _connectionService = connectionService;
        _chatService = chatService;
    }

    /// <summary>
    /// Метод получает список диалогов.
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен.</param>    
    /// <param name="projectId">Id проекта. Если не передан, то получает все диалоги пользователя.</param>
    /// <returns>Список диалогов.</returns>
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
            
            result.Dialogs = await CreateDialogMessagesBuilder.CreateDialogAsync((dialogs, mapDialogs), _chatRepository,
                _userRepository, userId, _mapper, account);
            
            var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

            await Clients
                .Client(connectionId)
                .SendAsync("listenGetProjectDialogs", result);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает диалог или создает новый и возвращает его.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен.</param>    
    /// <param name="dialogInput">Входная модель.</param>
    /// <returns>Данные диалога.</returns>
    public async Task GetDialogAsync(string account, string token, string dialogInput)
    {
        try
        {
            var json = JsonConvert.DeserializeObject<DialogInput>(dialogInput);
            var result = new DialogResultOutput
            {
                Errors = new List<ValidationFailure>()
            };
            var validator = await new GetDialogValidator().ValidateAsync(json);
            
            var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

            // Если есть ошибки, то вернем их.
            if (validator.Errors.Any())
            {
                result.Errors.AddRange(validator.Errors);
                
                await Clients
                    .Client(connectionId)
                    .SendAsync("listenGetDialog", result);

                return;
            }
            
            Enum.TryParse(json!.DiscussionType, out DiscussionTypeEnum discussionType);

            result = await _chatService.GetDialogAsync(json.DialogId, discussionType, account,
                json.DiscussionTypeId);
            result.ActionType = DialogActionType.Concrete.ToString();

            await Clients
                .Client(connectionId)
                .SendAsync("listenGetDialog", result);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод отправляет сообщение.
    /// </summary>
    /// <param name="messageInput">Входная модель.</param>
    /// <returns>Данные диалога с сообщениями. Обновляет диалог и сообщения диалога у всех участников диалога</returns>
    public async Task SendMessageAsync(string message, long dialogId, string account, string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }
            
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }

            var result = await _chatService.SendMessageAsync(message, dialogId, account);
            result.ActionType = DialogActionType.Message.ToString();
            
            var dialogRedis = await _connectionService.GetDialogMembersConnectionIdsCacheAsync(dialogId.ToString());
            var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

            // В кэше нет данных, будем добавлять текущего пользователя как первого.
            if (dialogRedis is null || !dialogRedis.Any())
            {
                // Добавляем текущего пользователя.
                dialogRedis = new List<DialogRedis>
                {
                    new()
                    {
                        Token = token,
                        ConnectionId = connectionId,
                        UserId = userId
                    }
                };
                
                await _connectionService.AddDialogMembersConnectionIdsCacheAsync(dialogId, dialogRedis);
            }

            // Нашли в кэше, будем проверять актуальность ConnectionId.
            else
            {
                var isActual = dialogRedis.Any(x => x.ConnectionId.Equals(connectionId));

                // Не актуален, обновим ConnectionId текущего пользователя.
                if (!isActual)
                {
                    var client = dialogRedis.Find(x => x.UserId == userId);

                    if (client is null)
                    {
                        throw new InvalidOperationException("Не удалось найти в кэше клиента. " +
                                                            $"DialogId: {dialogId}." +
                                                            $"UserId: {userId}");
                    }

                    // Если не актуален ConnectionId, то обновим на актуальный.
                    if (!client.ConnectionId.Equals(connectionId))
                    {
                        client.ConnectionId = connectionId;   
                    }

                    await _connectionService.AddDialogMembersConnectionIdsCacheAsync(dialogId, dialogRedis);
                }
            }

            var clients = dialogRedis.Select(x => x.ConnectionId).ToList();
            
            await Clients
                .Clients(clients)
                .SendAsync("listenSendMessage", result);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список диалогов для ЛК.
    /// </summary>
    /// <returns>Список диалогов.</returns>
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
                .SendAsync("listenProfileDialogs", result);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}