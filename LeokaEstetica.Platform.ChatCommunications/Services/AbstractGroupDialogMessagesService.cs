using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Communications.Abstractions;
using LeokaEstetica.Platform.Database.Abstractions.Communications;
using LeokaEstetica.Platform.Models.Dto.Communications.Output;

namespace LeokaEstetica.Platform.Communications.Services;

/// <summary>
/// Класс реализует методы сервиса сообщений диалога группы объектов абстрактной области чата.
/// </summary>
internal sealed class AbstractGroupDialogMessagesService : IAbstractGroupDialogMessagesService
{
    private readonly IAbstractGroupObjectsRepository _abstractGroupObjectsRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="abstractGroupObjectsRepository">Репозиторий объектов.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    public AbstractGroupDialogMessagesService(IAbstractGroupObjectsRepository abstractGroupObjectsRepository,
        IUserRepository userRepository)
    {
        _abstractGroupObjectsRepository = abstractGroupObjectsRepository;
        _userRepository = userRepository;
    }

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
}