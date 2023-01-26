using LeokaEstetica.Platform.Models.Entities.Subscription;

namespace LeokaEstetica.Platform.Database.Abstractions.Subscription;

/// <summary>
/// Абстракция репозитория подписок.
/// </summary>
public interface ISubscriptionRepository
{
    /// <summary>
    /// Метод получает список подписок.
    /// </summary>
    /// <returns>Список подписок.</returns>
    Task<List<SubscriptionEntity>> GetSubscriptionsAsync();

    /// <summary>
    /// Метод проставляет выделенные подписки пользователю.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список подписок, но с выделенной подпиской, которую оформил пользователь либо не выделяем.</returns>
    Task<List<long>> GetFillSubscriptionsAsync(long userId);
}