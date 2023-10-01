using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Models.Dto.Output.Project;

namespace LeokaEstetica.Platform.Services.Abstractions.Project;

/// <summary>
/// Абстракция сервиса выделение цветом пользователей.
/// </summary>
public interface IFillColorProjectsService
{
    /// <summary>
    /// Метод выделяет цветом пользователей у которых есть подписка выше бизнеса.
    /// </summary>
    /// <param name="projects">каталог проектов.</param>
    /// <param name="subscriptionRepository">Сервис подписок.</param>
    /// <param name="fareRuleRepository">Сервис правил тарифа.</param>
    /// <returns>Каталог проектов с выделеным цветом пользователей.</returns>
    Task<IEnumerable<CatalogProjectOutput>> SetColorBusinessProjectsAsync(List<CatalogProjectOutput> projects,
        ISubscriptionRepository subscriptionRepository, IFareRuleRepository fareRuleRepository);
}
