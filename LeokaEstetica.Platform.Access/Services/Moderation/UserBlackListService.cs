using LeokaEstetica.Platform.Access.Abstractions.Moderation;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Access;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Access;

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