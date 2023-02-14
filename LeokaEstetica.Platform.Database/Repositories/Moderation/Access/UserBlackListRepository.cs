using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Access;
using LeokaEstetica.Platform.Models.Entities.Access;

namespace LeokaEstetica.Platform.Database.Repositories.Moderation.Access;

/// <summary>
/// Класс репозитория ЧС пользователей.
/// </summary>
public class UserBlackListRepository : IUserBlackListRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public UserBlackListRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод добавляет пользователя в ЧС.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="email">Почта для блока..</param>
    /// <param name="phoneNumber">Номер телефона для блока.</param>
    public async Task AddUserBlackListAsync(long userId, string email, string phoneNumber)
    {
        await _pgContext.UserEmailBlackList.AddAsync(new UserEmailBlackListEntity
        {
            UserId = userId,
            Email = email
        });
        
        await _pgContext.UserPhoneBlackList.AddAsync(new UserPhoneBlackListEntity
        {
            UserId = userId,
            PhoneNumber = phoneNumber
        });

        await _pgContext.SaveChangesAsync();
    }
}