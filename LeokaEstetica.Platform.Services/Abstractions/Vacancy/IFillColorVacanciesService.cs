using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

namespace LeokaEstetica.Platform.Services.Abstractions.Vacancy;

/// <summary>
/// Абстракция сервиса выделение цветом пользователей.
/// </summary>
public interface IFillColorVacanciesService
{
    /// <summary>
    /// Метод выделяет цветом пользователей у которых есть подписка выше бизнеса.
    /// </summary>
    /// /// <param name="vacancies">каталог вакансий.</param>
    /// <param name="subscriptionRepository">Сервис подписок.</param>
    /// <param name="fareRuleRepository">Сервис правил тарифа.</param>
    /// <returns>Каталог вакансий с выделеным цветом цветом пользователей.</returns>
    Task<List<CatalogVacancyOutput>> SetColorBusinessVacancies(List<CatalogVacancyOutput> vacancies,
        ISubscriptionRepository subscriptionRepository, IFareRuleRepository fareRuleRepository);
}
