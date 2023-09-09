using AutoMapper;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Database.Chat;
using LeokaEstetica.Platform.Messaging.Builders;
using LeokaEstetica.Platform.Models.Dto.Chat.Output;
using LeokaEstetica.Platform.Redis.Abstractions.Connection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Messaging.Hubs;

/// <summary>
/// TODO: Если не заработает, то убрать internal.
/// Класс логики хаба для чатов.
/// </summary>
internal class ChatHub : Hub
{
    private readonly IChatRepository _chatRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ChatHub> _logger;
    private readonly IConnectionService _connectionService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="chatRepository">Репозиторий чата.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="connectionService">Сервис подключений Redis.</param>
    public ChatHub(IChatRepository chatRepository,
        IMapper mapper,
        IUserRepository userRepository,
        ILogger<ChatHub> logger,
        IConnectionService connectionService)
    {
        _chatRepository = chatRepository;
        _mapper = mapper;
        _userRepository = userRepository;
        _logger = logger;
        _connectionService = connectionService;
    }

    public async Task GetDialogsAsync(string account, string token, long? projectId = null)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }

            var dialogs = await _chatRepository.GetDialogsAsync(userId, projectId);
            var mapDialogs = _mapper.Map<List<ProfileDialogOutput>>(dialogs);
            
            dialogs = await CreateDialogMessagesBuilder.CreateDialogAsync((dialogs, mapDialogs), _chatRepository,
                _userRepository, userId, _mapper);
            
            var connectionId = await _connectionService.GetConnectionIdCacheAsync(token);

            await Clients
                .Client(connectionId)
                .SendAsync("GetDialogsAsync", dialogs);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}