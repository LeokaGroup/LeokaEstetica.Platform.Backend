using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Services.Abstractions.Vacancy;

namespace LeokaEstetica.Platform.Services.Services.Vacancy;

/// <summary>
/// Класс реализует методы сервиса выделение цветом пользователей.
/// </summary>
public class FillColorVacanciesService : IFillColorVacanciesService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IFareRuleRepository _fareRuleRepository;

    /// <summary>
    /// Список названий тарифов, которые дают выделение цветом.
    /// </summary>
    private static readonly List<string> _fareRuleTypesNames = new()
    {
        FareRuleTypeEnum.Business.GetEnumDescription(),
        FareRuleTypeEnum.Professional.GetEnumDescription()
    };

    public FillColorVacanciesService(ISubscriptionRepository subscriptionRepository,IFareRuleRepository fareRuleRepository )
    {
        _subscriptionRepository = subscriptionRepository;
        _fareRuleRepository = fareRuleRepository;
    }

    public async Task SetColorBusinessVacanciesAsync(IEnumerable<CatalogVacancyOutput> vacancies)
    {
        // Получаем список юзеров для проставления цветов.
        var userIds = vacancies.Select(p => p.UserId).Distinct();

        // Выбираем список подписок пользователей.
        var userSubscriptions = await _subscriptionRepository.GetUsersSubscriptionsAsync(userIds);

        // Получаем список подписок.
        var subscriptions = await _subscriptionRepository.GetSubscriptionsAsync();

        // Получаем список тарифов, чтобы взять названия тарифов.
        var fareRules = await _fareRuleRepository.GetFareRulesAsync();
        //var fareRulesList = fareRules.ToList();

        //Выбираем пользователей, у которых есть подписка выше бизнеса.Только их выделяем цветом.
        foreach (var vacancy in vacancies)
        {
            // Смотрим подписку пользователя.
            var userSubscription = userSubscriptions.Find(s => s.UserId == vacancy.UserId);
            var subscription = subscriptions.Find(s => s.ObjectId == userSubscription?.SubscriptionId);

            // Получаем название тарифа подписки.
            var fareRule = fareRules.ToList().Find(fr => fr.RuleId == subscription?.ObjectId);

            if (userSubscription is null || subscription is null || fareRule is null)
            {
                continue;
            }

            // Подписка позволяет. Проставляем выделение цвета.
            if (_fareRuleTypesNames.Contains(fareRule.Name))
            {
                vacancy.IsSelectedColor = true;
            }
        }
    }
}
