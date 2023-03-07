namespace LeokaEstetica.Platform.Database.Access.User;

/// <summary>
/// Абстракция репозитория проверки доступа пользователей. 
/// </summary>
public interface IAccessUserRepository
{
    /// <summary>
    /// Метод проверяет блокировку пользователя по параметру, который передали.
    /// Поочередно проверяем по почте, номеру телефона.
    /// </summary>
    /// <param name="availableBlockedText">Почта или номер телефона для проверки блокировки.</param>
    /// <returns>Признак блокировки.</returns>
    Task<bool> CheckBlockedUserAsync(string availableBlockedText);
}