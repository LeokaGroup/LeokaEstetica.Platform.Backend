using LeokaEstetica.Platform.Models.Dto.Output.Subscription;
using LeokaEstetica.Platform.Models.Entities.Subscription;

namespace LeokaEstetica.Platform.Services.Abstractions.Subscription;

/// <summary>
/// Абстракция сервиса подписок.
/// </summary>
public interface ISubscriptionService
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
    /// <param name="subscriptions">Список подписок до выделения.</param>
    /// <returns>Список подписок, но с выделенной подпиской, которую оформил пользователь либо не выделяем.</returns>
    Task<List<SubscriptionOutput>> FillSubscriptionsAsync(string account, List<SubscriptionOutput> subscriptions);
}