using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Entities.FareRule;
using LeokaEstetica.Platform.Models.Entities.Subscription;

namespace LeokaEstetica.Platform.Services.Abstractions.Project;

/// <summary>
/// Абстракция сервиса выделение цветом пользователей.
/// </summary>
public interface IFillColorProjectsService
{
    /// <summary>
    /// Метод выделяет цветом пользователей у которых есть подписка выше бизнеса.
    /// </summary>
    public void SetColorBusinessProjects(ref IEnumerable<CatalogProjectOutput> projects,
        List<UserSubscriptionEntity> userSubscriptions,
        List<SubscriptionEntity> subscriptions,
        List<FareRuleEntity> fareRulesList);
}
