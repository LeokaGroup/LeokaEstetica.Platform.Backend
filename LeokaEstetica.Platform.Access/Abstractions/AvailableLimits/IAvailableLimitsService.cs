using LeokaEstetica.Platform.Access.Models.Output;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;

namespace LeokaEstetica.Platform.Access.Abstractions.AvailableLimits;

/// <summary>
/// Абстракция сервиса проверки лимитов.
/// </summary>
public interface IAvailableLimitsService
{
    /// <summary>
    /// Метод проверяет, доступны ли пользователю для создания проекты в зависимости от подписки. 
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="fareRuleName">Название тарифа.</param>
    /// <returns>Признак доступости.</returns>
    Task<bool> CheckAvailableCreateProjectAsync(long userId, string fareRuleName);
    
    /// <summary>
    /// Метод проверяет, доступны ли пользователю для создания вакансии в зависимости от подписки. 
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="fareRuleName">Название тарифа.</param>
    /// <returns>Признак доступости.</returns>
    Task<bool> CheckAvailableCreateVacancyAsync(long userId, string fareRuleName);

    /// <summary>
    /// Метод проверяет, проходит ли понижающий тариф по лимитам при переходе на него с более дорогого тарифа.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="publicId">Публичный ключ тарифа, на который переходят.</param>
    /// <returns>Результирующая модель с результатами по лимитам.</returns>
    Task<ReductionSubscriptionLimitsOutput> CheckAvailableReductionSubscriptionAsync(long userId, Guid publicId,
        ISubscriptionRepository _subscriptionRepository, IFareRuleRepository fareRuleRepository);
}