using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Access.User;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Access.User;

/// <summary>
/// Класс реализует методы репозитория проверки доступа пользователей. 
/// </summary>
public class AccessUserRepository : IAccessUserRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public AccessUserRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод проверяет блокировку пользователя по параметру, который передали.
    /// Поочередно проверяем по почте, номеру телефона.
    /// </summary>
    /// <param name="availableBlockedText">Почта или номер телефона для проверки блокировки.</param>
    /// <returns>Признак блокировки.</returns>
    public async Task<bool> CheckBlockedUserAsync(string availableBlockedText)
    {
        // Проверяем блокировку пользрвателя по почте.
        var blockedEmailUser = await _pgContext.UserEmailBlackList
            .AnyAsync(u => u.Email.Equals(availableBlockedText));

        if (blockedEmailUser)
        {
            return true;
        }
        
        // Проверяем блокировку пользрвателя по номеру телефона.
        var blockedUserPhoneNumber = await _pgContext.UserPhoneBlackList
            .AnyAsync(u => u.PhoneNumber.Equals(availableBlockedText));
        
        if (blockedUserPhoneNumber)
        {
            return true;
        }

        return false;
    }
}