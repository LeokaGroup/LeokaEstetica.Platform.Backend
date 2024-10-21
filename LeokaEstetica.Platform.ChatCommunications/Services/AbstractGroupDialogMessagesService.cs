using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Base.Models.IntegrationEvents.Communications;
using LeokaEstetica.Platform.Communications.Abstractions;
using LeokaEstetica.Platform.Database.Abstractions.Communications;
using LeokaEstetica.Platform.Models.Dto.Communications.Output;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.RabbitMq.Abstractions;

namespace LeokaEstetica.Platform.Communications.Services;

/// <summary>
/// Класс реализует методы сервиса сообщений диалога группы объектов абстрактной области чата.
/// </summary>
internal sealed class AbstractGroupDialogMessagesService : IAbstractGroupDialogMessagesService
{
    private readonly IAbstractGroupObjectsRepository _abstractGroupObjectsRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="abstractGroupObjectsRepository">Репозиторий объектов.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="rabbitMqService">Сервис RabbitMQ.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    public AbstractGroupDialogMessagesService(IAbstractGroupObjectsRepository abstractGroupObjectsRepository,
        IUserRepository userRepository,
        IRabbitMqService rabbitMqService,
        IConfiguration configuration)
    {
        _abstractGroupObjectsRepository = abstractGroupObjectsRepository;
        _userRepository = userRepository;
        _rabbitMqService = rabbitMqService;
        _configuration = configuration;
    }

    #region Публичные методы.

    /// <inheritdoc />
    public async Task<IEnumerable<GroupObjectDialogMessageOutput>> GetObjectDialogMessagesAsync(long dialogId,
        string account)
    {
        var userId = await _userRepository.GetUserByEmailAsync(account);

        if (userId <= 0)
        {
            throw new InvalidOperationException("Ошибка определения пользователя при получении сообщений диалога. " +
                                                $"UserId: {userId}.");
        }
        
        var result = await _abstractGroupObjectsRepository.GetObjectDialogMessagesAsync(dialogId, userId);

        return result;
    }

    /// <inheritdoc />
    public async Task SendMessageToQueueAsync(string? message, long dialogId, string account, Guid userCode,
        UserConnectionModuleEnum module)
    {
        var userId = await _userRepository.GetUserByEmailAsync(account);

        if (userId <= 0)
        {
            throw new InvalidOperationException("Ошибка определения пользователя при получении сообщений диалога. " +
                                                $"UserId: {userId}.");
        }
        
        var queueType = string.Empty.CreateQueueDeclareNameFactory(_configuration["Environment"],
            QueueTypeEnum.DialogMessages);

        await _rabbitMqService.PublishAsync(new DialogMessageEvent
        {
            CreatedBy = userId,
            Message = message,
            DialogId = dialogId,
            UserCode = userCode,
            Module = module
        }, queueType, _configuration);
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}