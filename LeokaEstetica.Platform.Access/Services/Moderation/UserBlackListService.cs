using LeokaEstetica.Platform.Access.Abstractions.Moderation;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Access;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Access;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;

namespace LeokaEstetica.Platform.Access.Services.Moderation;

/// <summary>
/// Класс реализует методы сервиса ЧС пользователей.
/// </summary>
public class UserBlackListService : IUserBlackListService
{
    private readonly ILogService _logService;
    private readonly IUserBlackListRepository _userBlackListRepository;
    private readonly INotificationsService _notificationsService;

    /// <summary>
    /// Конструктор.
    /// <param name="logService">Сервис логера.</param>
    /// <param name="userBlackListRepository">Репозиторий ЧС пользователей.</param>
    /// <param name="notificationsService">Сервис уведомлений.</param>
    /// </summary>
    public UserBlackListService(ILogService logService, 
        IUserBlackListRepository userBlackListRepository,
        INotificationsService notificationsService)
    {
        _logService = logService;
        _userBlackListRepository = userBlackListRepository;
        _notificationsService = notificationsService;
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

    /// <summary>
    /// Метод удаляет пользователя из ЧС.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task RemoveUserBlackListAsync(long userId, string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException("Пользователя не существует");

            var exist = await _userBlackListRepository.IsUserExistAsync(userId);

            if (!exist)
                throw new ArgumentNullException("Пользователя в чс не существует.");
                
            await _userBlackListRepository.RemoveUserBlackListAsync(userId);
            await _notificationsService.SendNotifySuccessUserRemovedBlackListAsync("Успешно.",
                "Пользователь был успешно удален из чёрного списка.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS,
                token);
        }

        catch(Exception ex) 
        {
            if (!string.IsNullOrEmpty(token))
            {
                await _notificationsService.SendNotifyWarningUserNotFoundBlackListAsync("Внимание.",
                "Возникла ошибка при удалении пользователя.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING,
                token);
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