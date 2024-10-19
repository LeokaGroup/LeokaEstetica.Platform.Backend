using LeokaEstetica.Platform.Access.Abstractions.Moderation;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Access;
using LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Access;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Access.Services.Moderation;

/// <summary>
/// Класс реализует методы сервиса ЧС пользователей.
/// </summary>
public class UserBlackListService : IUserBlackListService
{
    private readonly ILogger<UserBlackListService> _logger;
    private readonly IUserBlackListRepository _userBlackListRepository;
    
    /// <summary>
    /// Конструктор.
    /// <param name="logger">Сервис логера.</param>
    /// <param name="userBlackListRepository">Репозиторий ЧС пользователей.</param>
    /// </summary>
    public UserBlackListService(ILogger<UserBlackListService> logger, 
        IUserBlackListRepository userBlackListRepository)
    {
        _logger = logger;
        _userBlackListRepository = userBlackListRepository;
    }

    /// <summary>
    /// Метод добавляет пользователя в ЧС.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="email">Почта для блока..</param>
    /// <param name="phoneNumber">Номер телефона для блока.</param>
    /// <param name="vkUserId">Id пользователя в системе ВКонтакте.</param>
    public async Task AddUserBlackListAsync(long userId, string? email, string? phoneNumber, string? vkUserId)
    {
        try
        {
            await _userBlackListRepository.AddUserBlackListAsync(userId, email, phoneNumber, vkUserId);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
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
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}