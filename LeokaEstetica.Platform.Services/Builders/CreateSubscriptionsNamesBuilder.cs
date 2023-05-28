using LeokaEstetica.Platform.Models.Dto.Output.Subscription;
using LeokaEstetica.Platform.Models.Entities.FareRule;

namespace LeokaEstetica.Platform.Services.Builders;

/// <summary>
/// Класс билдера создает названий подписок.
/// </summary>
public static class CreateSubscriptionsNamesBuilder
{
    /// <summary>
    /// Метод записывает названия подписок.
    /// </summary>
    /// <param name="subscriptions">Список подписок.</param>
    public static void Create(ref List<SubscriptionOutput> subscriptions, List<FareRuleEntity> fareRules)
    {
        foreach (var s in subscriptions)
        {
            var fr = fareRules.FirstOrDefault(fr => fr.RuleId == s.ObjectId); // Находим тариф.
            s.SubscriptionName = fr?.Name; // Записываем название подписке.
        }
    }
}