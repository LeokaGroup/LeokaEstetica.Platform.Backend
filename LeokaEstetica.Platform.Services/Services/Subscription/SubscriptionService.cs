using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Models.Dto.Output.Subscription;
using LeokaEstetica.Platform.Models.Entities.Subscription;
using LeokaEstetica.Platform.Services.Abstractions.Subscription;
using LeokaEstetica.Platform.Services.Builders;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Services.Services.Subscription;

/// <summary>
/// Класс реализует методы сервиса подписок.
/// </summary>
public class SubscriptionService : ISubscriptionService
{
    private readonly ILogger<SubscriptionService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IFareRuleRepository _fareRuleRepository;

    public SubscriptionService(ILogger<SubscriptionService> logger,
        IUserRepository userRepository,
        ISubscriptionRepository subscriptionRepository, 
        IFareRuleRepository fareRuleRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _subscriptionRepository = subscriptionRepository;
        _fareRuleRepository = fareRuleRepository;
    }

    /// <summary>
    /// Метод получает список подписок.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список подписок.</returns>
    public async Task<List<SubscriptionEntity>> GetSubscriptionsAsync()
    {
        try
        {
            var result = await _subscriptionRepository.GetSubscriptionsAsync();

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод проставляет выделенные подписки пользователю.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="subscriptions">Список подписок до выделения.</param>
    /// <returns>Список подписок, но с выделенной подпиской, которую оформил пользователь либо не выделяем.</returns>
    public async Task<List<SubscriptionOutput>> FillSubscriptionsAsync(string account,
        List<SubscriptionOutput> subscriptions)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                throw new NotFoundUserIdByAccountException(account);
            }

            // Получаем подписки пользователя.
            var userSubscriptions = await _subscriptionRepository.GetFillSubscriptionsAsync(userId);

            // Если пользователь не оформлял подписок, то и выделять нечего.
            if (userSubscriptions.Any())
            {
                // Проставляем выделение подпискам.
                FillSubscriptionsBuilder.Fill(ref subscriptions, userSubscriptions);
            }

            // Записываем названия.
            var ids = subscriptions.Select(s => s.ObjectId);
            var fareRules = await _fareRuleRepository.GetFareRulesNamesByIdsAsync(ids);
            CreateSubscriptionsNamesBuilder.Create(ref subscriptions, fareRules);

            return subscriptions;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}