using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Models.Entities.FareRule;
using LeokaEstetica.Platform.Models.Entities.Subscription;

namespace LeokaEstetica.Platform.Services.Abstractions.Vacancy;

/// <summary>
/// Абстракция сервиса выделение цветом пользователей.
/// </summary>
public interface IFillColorVacanciesService
{
    /// <summary>
    /// Метод выделяет цветом пользователей у которых есть подписка выше бизнеса и возвращает измененный каталог.
    /// </summary>
    public Task<IEnumerable<CatalogVacancyOutput>> SetColorBusinessVacanciesAsync(IEnumerable<CatalogVacancyOutput> vacancies,
        List<UserSubscriptionEntity> userSubscriptions,
        List<SubscriptionEntity> subscriptions,
        List<FareRuleEntity> fareRulesList);
}
