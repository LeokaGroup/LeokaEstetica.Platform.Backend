using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Services.Abstractions.Project;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Services.Services.Project;

/// <summary>
/// TODO: Выпилить сервис, если у нас не будет выделения цветами тарифов.
/// Класс реализует методы сервиса выделение цветом пользователей.
/// </summary>
internal sealed class FillColorProjectsService : IFillColorProjectsService
{
    /// <summary>
    /// Список названий тарифов, которые дают выделение цветом.
    /// </summary>
    private static readonly List<string> _fareRuleTypesNames = new()
    {
        FareRuleTypeEnum.Business.GetEnumDescription(),
        FareRuleTypeEnum.Professional.GetEnumDescription()
    };

    /// <summary>
    /// Метод выделяет цветом пользователей у которых есть подписка выше бизнеса.
    /// </summary>
    // public async Task<IEnumerable<CatalogProjectOutput>> SetColorBusinessProjectsAsync(List<CatalogProjectOutput> projects,
    //     ISubscriptionRepository subscriptionRepository, IFareRuleRepository fareRuleRepository)
    //
    // {
    //     // Получаем список юзеров для проставления цветов.
    //     var userIds = projects.Select(p => p.UserId).Distinct();
    //
    //     // Получаем список подписок.
    //     var subscriptions = await subscriptionRepository.GetSubscriptionsAsync();
    //
    //     // Получаем список тарифов, чтобы взять названия тарифов.
    //     var fareRules = await fareRuleRepository.GetFareRulesAsync();
    //     var rules = fareRules.ToList();
    //     
    //     // Выбираем список подписок пользователей.
    //     var userSubscriptions = await subscriptionRepository.GetUsersSubscriptionsAsync(userIds);
    //
    //     // Выбираем пользователей, у которых есть подписка выше бизнеса. Только их выделяем цветом.
    //     foreach (var project in projects)
    //     {
    //         // Смотрим подписку пользователя.
    //         var userSubscription = userSubscriptions.Find(s => s.UserId == project.UserId);
    //
    //         if (userSubscription is null)
    //         {
    //             continue;
    //         }
    //         
    //         var subscription = subscriptions.Find(s => s.ObjectId == userSubscription.SubscriptionId);
    //
    //         if (subscription is null)
    //         {
    //             continue;
    //         }
    //
    //         // Получаем название тарифа подписки.
    //         var fareRule = rules.Find(fr => fr.RuleId == subscription.ObjectId);
    //
    //         if (fareRule is null)
    //         {
    //             continue;
    //         }
    //
    //         // Подписка позволяет. Проставляем выделение цвета.
    //         if (_fareRuleTypesNames.Contains(fareRule.Name))
    //         {
    //             project.IsSelectedColor = true;
    //         }
    //     }
    //     
    //     return projects;
    // }
}
