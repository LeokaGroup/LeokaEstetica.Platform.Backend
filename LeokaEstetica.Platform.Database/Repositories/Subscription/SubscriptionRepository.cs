using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Models.Entities.Subscription;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Subscription;

/// <summary>
/// Класс реализует методы репозитория подписок.
/// </summary>
public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly PgContext _pgContext;
    
    public SubscriptionRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод получает список подписок.
    /// </summary>
    /// <returns>Список подписок.</returns>
    public async Task<List<SubscriptionEntity>> GetSubscriptionsAsync()
    {
        var result = await _pgContext.Subscriptions.ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод проставляет выделенные подписки пользователю.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список подписок, но с выделенной подпиской, которую оформил пользователь либо не выделяем.</returns>
    public async Task<List<long>> GetFillSubscriptionsAsync(long userId)
    {
        var reult = await _pgContext.UserSubscriptions
            .Where(s => s.UserId == userId)
            .Select(s => s.SubscriptionId)
            .ToListAsync();

        return reult;
    }

    /// <summary>
    /// Метод получает подписку пользователя по его Id.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Число, означающее уровень доступа.</returns>
    public async Task<SubscriptionEntity> GetUserSubscriptionAsync(long userId)
    {
        // Получаем активную подписку пользователя.
        var userSubscription = await _pgContext.UserSubscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId 
                                      && s.IsActive);

        // Доступа нет.
        if (userSubscription is null)
        {
            return null;
        }
        
        var result = await _pgContext.Subscriptions
            .FirstOrDefaultAsync(s => s.SubscriptionId == userSubscription.SubscriptionId);

        return result;
    }
}