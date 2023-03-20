using LeokaEstetica.Platform.Models.Entities.Profile;

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
    /// <param name="isVkAuth">Признак блокировки через ВК.</param>
    /// <returns>Признак блокировки.</returns>
    Task<bool> CheckBlockedUserAsync(string availableBlockedText, bool isVkAuth);
    
    /// <summary>
    /// Метод проверяет, заполнена ли анкета пользователя.
    /// Если не заполнена, то запрещаем доступ к ключевому функционалу.
    /// </summary>
    /// <param name="userId">ID пользователя.</param>
    /// <returns>Признак проверки.</returns>
    Task<ProfileInfoEntity> IsProfileEmptyAsync(long userId);
}