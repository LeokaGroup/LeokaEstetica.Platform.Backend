using LeokaEstetica.Platform.Core.Enums;
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
    /// Метод получает выделенные подписки пользователю.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список подписок, но с выделенной подпиской, которую оформил пользователь либо не выделяем.</returns>
    Task<List<long>> GetFillSubscriptionsAsync(long userId);

    /// <summary>
    /// Метод получает подписку пользователя по его Id.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Подписку.</returns>
    Task<SubscriptionEntity> GetUserSubscriptionAsync(long userId);

    /// <summary>
    /// Метод запишет пользователю подписку.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="subscriptionType">Тип подписки.</param>
    /// <param name="objectId">Id типа подписки.</param>
    Task AddUserSubscriptionAsync(long userId, SubscriptionTypeEnum subscriptionType, long objectId);
}