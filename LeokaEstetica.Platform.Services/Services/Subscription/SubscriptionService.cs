using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Models.Dto.Output.Subscription;
using LeokaEstetica.Platform.Models.Entities.Subscription;
using LeokaEstetica.Platform.Services.Abstractions.Subscription;
using LeokaEstetica.Platform.Services.Builders;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Services.Services.Subscription;

/// <summary>
/// Класс реализует методы сервиса подписок.
/// </summary>
internal sealed class SubscriptionService : ISubscriptionService
{
    private readonly ILogger<SubscriptionService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IFareRuleRepository _fareRuleRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="subscriptionRepository">Репозиторий подписок.</param>
    /// <param name="fareRuleRepository">Репозиторий тарифов.</param>
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

    #region Публичные методы.

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

    /// <summary>
    /// Метод запишет пользователю подписку.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="month">Кол-во месяцев подписки.</param>
    /// <param name="orderId">Id заказа.</param>
    /// <param name="ruleId">Id тарифа.</param>
    public async Task SetUserSubscriptionAsync(long userId, Guid publicId, short? month, long orderId, int ruleId)
    {
        if (month <= 0)
        {
            var ex = new InvalidOperationException(
                "Для присвоения подписки кол-во месяцев не может быть отрицательным." +
                $"Кол-во было: {month}" +
                $"PublicId тарифа: {publicId}" +
                $" UserId: {userId}." +
                $" OrderId: {orderId}");
            throw ex;
        }

        // Id тарифа должен быть > 0, так как если он 1, то это бесплатный тариф. Если он 0, то это ошибка.
        // А его оформлять не нужно, так при регистрации у пользователя и так присваивается бесплатная.
        if (ruleId <= 1)
        {
            throw new InvalidOperationException(
                "Для присвоения подписки пользователю должен быть оплачен один из платных тарифов." +
                $"PublicId тарифа: {publicId}" +
                $" UserId: {userId}." +
                $" OrderId: {orderId}");
        }
        
        var days = 0;
        var currentYear = DateTime.UtcNow.Year;
        var calcMonth = month; // Кол-во мес. которые используем в вычислениях, не меняя исходное кол-во мес.
        
        while (calcMonth > 0)
        {
            // Суммируем дни от каждого месяца.
            days += DateTime.DaysInMonth(currentYear, calcMonth.Value);
            calcMonth--;
        }

        // Проставляем подписку пользователю.
        await _userRepository.SetSubscriptionAsync(ruleId, userId, month.Value, days);
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}