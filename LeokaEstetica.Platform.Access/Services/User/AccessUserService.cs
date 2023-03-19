using LeokaEstetica.Platform.Access.Abstractions.User;
using LeokaEstetica.Platform.Database.Access.User;

namespace LeokaEstetica.Platform.Access.Services.User;

/// <summary>
/// Класс реализует методы сервиса проверки доступа пользователей.
/// </summary>
public class AccessUserService : IAccessUserService
{
    private readonly IAccessUserRepository _accessUserRepository;
    
    public AccessUserService(IAccessUserRepository accessUserRepository)
    {
        _accessUserRepository = accessUserRepository;
    }

    /// <summary>
    /// Метод проверяет блокировку пользователя по параметру, который передали.
    /// Поочередно проверяем по почте, номеру телефона.
    /// </summary>
    /// <param name="availableBlockedText">Почта или номер телефона для проверки блокировки.</param>
    /// <param name="isVkAuth">Признак блокировки через ВК.</param>
    /// <returns>Признак блокировки.</returns>
    public async Task<bool> CheckBlockedUserAsync(string availableBlockedText, bool isVkAuth)
    {
        var blockedUser = await _accessUserRepository.CheckBlockedUserAsync(availableBlockedText, isVkAuth);

        return blockedUser;
    }
}