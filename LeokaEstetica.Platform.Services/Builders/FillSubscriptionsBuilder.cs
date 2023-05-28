using LeokaEstetica.Platform.Models.Dto.Output.Subscription;

namespace LeokaEstetica.Platform.Services.Builders;

/// <summary>
/// Класс билдера проставляет выделение подпискам.
/// </summary>
public static class FillSubscriptionsBuilder
{
    /// <summary>
    /// Метод проставляет выделение подпискам.
    /// </summary>
    /// <param name="subscriptions">Список подписок.</param>
    /// <param name="userSubscriptions">Список подписок пользователя.</param>
    public static void Fill(ref List<SubscriptionOutput> subscriptions, List<long> userSubscriptions)
    {
        foreach (var s in subscriptions)
        {
            if (userSubscriptions.Contains(s.SubscriptionId))
            {
                s.IsActive = true;
            }
        }
    }
}