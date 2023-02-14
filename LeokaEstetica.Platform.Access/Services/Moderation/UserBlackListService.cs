using LeokaEstetica.Platform.Access.Abstractions.Moderation;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Access;
using LeokaEstetica.Platform.Logs.Abstractions;

namespace LeokaEstetica.Platform.Access.Services.Moderation;

/// <summary>
/// Класс реализует методы сервиса ЧС пользователей.
/// </summary>
public class UserBlackListService : IUserBlackListService
{
    private readonly ILogService _logService;
    private readonly IUserBlackListRepository _userBlackListRepository;
    
    /// <summary>
    /// Конструктор.
    /// <param name="logService">Сервис логера.</param>
    /// <param name="userBlackListRepository">Репозиторий ЧС пользователей.</param>
    /// </summary>
    public UserBlackListService(ILogService logService, 
        IUserBlackListRepository userBlackListRepository)
    {
        _logService = logService;
        _userBlackListRepository = userBlackListRepository;
    }

    /// <summary>
    /// Метод добавляет пользователя в ЧС.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="email">Почта для блока..</param>
    /// <param name="phoneNumber">Номер телефона для блока.</param>
    public async Task AddUserBlackListAsync(long userId, string email, string phoneNumber)
    {
        try
        {
            await _userBlackListRepository.AddUserBlackListAsync(userId, email, phoneNumber);
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }
}