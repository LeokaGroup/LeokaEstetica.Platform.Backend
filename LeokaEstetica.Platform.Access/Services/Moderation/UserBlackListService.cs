using LeokaEstetica.Platform.Access.Abstractions.Moderation;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Access;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Access;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;

namespace LeokaEstetica.Platform.Access.Services.Moderation;

/// <summary>
/// Класс реализует методы сервиса ЧС пользователей.
/// </summary>
public class UserBlackListService : IUserBlackListService
{
    private readonly ILogService _logService;
    private readonly IUserRepository _userRepository;
    private readonly IUserBlackListRepository _userBlackListRepository;
    private readonly INotificationsService _notificationService;

    /// <summary>
    /// Конструктор.
    /// <param name="logService">Сервис логера.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="userBlackListRepository">Репозиторий ЧС пользователей.</param>
    /// <param name="notificationService">Сервис уведомлений.</param>
    /// </summary>
    public UserBlackListService(ILogService logService, 
        IUserRepository userRepository,
        IUserBlackListRepository userBlackListRepository,
        INotificationsService notificationService)
    {
        _logService = logService;
        _userRepository = userRepository;
        _userBlackListRepository = userBlackListRepository;
        _notificationService = notificationService;
    }

    /// <summary>
    /// Метод добавляет пользователя в ЧС.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="email">Почта для блока..</param>
    /// <param name="phoneNumber">Номер телефона для блока.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task AddUserBlackListAsync(long userId, string email, string phoneNumber, string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Пользователя не существует");

            var user = await _userRepository.GetUserByUserIdAsync(userId);

            if (user == null)
                throw new InvalidOperationException($"Пользователя не существует. UserId {userId}");

            var userBlocked = await _userBlackListRepository.IsUserBlocked(userId);

            if (userBlocked)
                throw new InvalidOperationException($"Пользователь уже добавлен в чс. UserId: {userId}");

            await _userBlackListRepository.AddUserBlackListAsync(userId, email, phoneNumber);
            await _notificationService.SendNotifySuccessBlockAsync("Все хорошо", "Пользователь успешно заблокирован.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);
        }
        
        catch (Exception ex)
        {
            if(!string.IsNullOrEmpty(token))
            {
                await _notificationService.SendNotifyWarningBlockAsync("Внимание", "Во время блокирования пользователя" +
                    " произошла ошибка.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);
            }

            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список пользователей в ЧС.
    /// </summary>
    /// <returns>Список пользователей в ЧС.</returns>
    public async Task<UserBlackListResult> GetUsersBlackListAsync()
    {
        try
        {
            var fullList = await _userBlackListRepository.GetUsersBlackListAsync();
            var result = new UserBlackListResult
            {
                UsersBlackList = from um in fullList.Item1
                    join un in fullList.Item2
                        on um.UserId
                        equals un.UserId
                        into temp
                    from tbl in temp.DefaultIfEmpty()
                    select new UserBlackListOutput
                    {
                        UserId = um.UserId > 0 ? um.UserId : tbl?.UserId ?? 0,
                        Email = um.Email,
                        PhoneNumber = tbl?.PhoneNumber
                    }
            };

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }
}