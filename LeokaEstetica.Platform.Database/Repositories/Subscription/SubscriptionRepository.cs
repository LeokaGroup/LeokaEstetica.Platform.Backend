using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Models.Dto.Output.Subscription;
using LeokaEstetica.Platform.Models.Entities.Subscription;
using LeokaEstetica.Platform.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Enum = LeokaEstetica.Platform.Models.Enums.Enum;

namespace LeokaEstetica.Platform.Database.Repositories.Subscription;

/// <summary>
/// Класс реализует методы репозитория подписок.
/// </summary>
internal sealed class SubscriptionRepository : BaseRepository, ISubscriptionRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public SubscriptionRepository(PgContext pgContext,
        IConnectionProvider connectionProvider) : base(connectionProvider)
    {
        _pgContext = pgContext;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает список подписок.
    /// </summary>
    /// <returns>Список подписок.</returns>
    public async Task<List<SubscriptionEntity>> GetSubscriptionsAsync()
    {
        var result = await _pgContext.Subscriptions.ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод проставляет выделенные подписки пользователю.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список подписок, но с выделенной подпиской, которую оформил пользователь либо не выделяем.</returns>
    public async Task<List<long>> GetFillSubscriptionsAsync(long userId)
    {
        var reult = await _pgContext.UserSubscriptions
            .Where(s => s.UserId == userId)
            .Select(s => s.SubscriptionId)
            .ToListAsync();

        return reult;
    }

    /// <summary>
    /// Метод получает подписку пользователя по его Id.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Число, означающее уровень доступа.</returns>
    public async Task<SubscriptionEntity> GetUserSubscriptionAsync(long userId)
    {
        // Получаем активную подписку пользователя.
        var userSubscription = await _pgContext.UserSubscriptions.FirstOrDefaultAsync(s => s.UserId == userId
            && s.IsActive);

        // Повторно пробуем добавить подписку пользователю.
        if (userSubscription is null)
        {
            await AddUserSubscriptionAsync(userId, SubscriptionTypeEnum.FareRule, 1);

            userSubscription = await _pgContext.UserSubscriptions.FirstOrDefaultAsync(s => s.UserId == userId
                && s.IsActive);
        }

        // Доступа нет.
        if (userSubscription is null)
        {
            return null;
        }
        
        var result = await _pgContext.Subscriptions
            .FirstOrDefaultAsync(s => s.SubscriptionId == userSubscription.SubscriptionId);

        return result;
    }

    /// <summary>
    /// Метод запишет пользователю подписку.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="subscriptionType">Тип подписки.</param>
    /// <param name="objectId">Id типа подписки.</param>
    public async Task AddUserSubscriptionAsync(long userId, SubscriptionTypeEnum subscriptionType, long objectId)
    {
        // Получаем подписку, которую надо присвоить пользователю.
        var freeSubscriptionId = await _pgContext.Subscriptions
            .Where(s => s.ObjectId == objectId
                        && s.SubscriptionType.Equals(subscriptionType.ToString()))
            .Select(s => s.SubscriptionId)
            .FirstOrDefaultAsync();

        // Присваиваем пользователю подписку.
        await _pgContext.UserSubscriptions.AddAsync(new UserSubscriptionEntity
        {
            UserId = userId,
            IsActive = true,
            SubscriptionId = freeSubscriptionId
        });
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод получает список подписок пользователей.
    /// </summary>
    /// <param name="userIds">Список Id пользователей.</param>
    /// <returns>Список подписок.</returns>
    public async Task<List<UserSubscriptionEntity>> GetUsersSubscriptionsAsync(IEnumerable<long> userIds)
    {
        var result = await _pgContext.UserSubscriptions
            .Where(s => userIds.Contains(s.UserId))
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает подписку пользователя.
    /// </summary>
    /// <param name="subscriptionId">Id подписки.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Подписка пользователя.</returns>
    public async Task<UserSubscriptionEntity> GetUserSubscriptionBySubscriptionIdAsync(long subscriptionId,
        long userId)
    {
        var result = await _pgContext.UserSubscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId
                                      && s.IsActive
                                      && s.SubscriptionId == subscriptionId);

        return result;
    }

    /// <summary>
    /// Метод делает подписку неактивной.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    public async Task DisableUserSubscriptionAsync(long userId)
    {
        var result = await _pgContext.UserSubscriptions.FirstOrDefaultAsync(s => s.UserId == userId);
        result!.IsActive = false;
        await _pgContext.SaveChangesAsync();
    }

    /// <summary>
    /// Метод автоматически присваивает аккаунту пользователя бесплатный тариф.
    /// Обновляет неактивную подписку через сброс на бесплатный.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    public async Task AutoDefaultUserSubscriptionAsync(long userId)
    {
        // Получаем неактивную подписку пользователя.
        var userSubscription = await _pgContext.UserSubscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (userSubscription is null)
        {
            throw new InvalidOperationException(
                "Не удалось получить подписку пользователя для сброса аккаунта до бесплатного тарифа." +
                $" UserId: {userId}");
        }
        
        // Получаем подписку, которую надо присвоить пользователю.
        var freeSubscriptionId = await _pgContext.Subscriptions
            .Where(s => s.ObjectId == 1
                        && s.SubscriptionType.Equals(SubscriptionTypeEnum.FareRule.ToString()))
            .Select(s => s.SubscriptionId)
            .FirstOrDefaultAsync();
        
        userSubscription.IsActive = true;
        userSubscription.SubscriptionId = freeSubscriptionId;

        await _pgContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<SubscriptionFareRuleCompositeOutput?> GetUserSubscriptionFareRuleByUserIdAsync(long userId,
        int attributeId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId);
        parameters.Add("@subscriptionType", SubscriptionTypeEnum.FareRule.ToString());
        parameters.Add("@attributeId", attributeId);

        var query = "SELECT asub.\"SubscriptionId\", fr.is_free, fra.is_price, fra.min_value " +
					"FROM \"Subscriptions\".\"UserSubscriptions\" AS us " +
					"INNER JOIN \"Subscriptions\".\"Subscriptions\" AS asub " +
					"ON us.\"SubscriptionId\" = asub.\"SubscriptionId\" " +
                    "INNER JOIN rules.fare_rules AS fr " +
					"ON asub.\"SubscriptionId\" = fr.rule_id " +
                    "INNER JOIN rules.fare_rule_attribute_values AS fra " +
                    "ON fr.rule_id = fra.rule_id " +
					"WHERE us.\"UserId\" = @userId " +
					"AND asub.\"SubscriptionType\" = @subscriptionType " +
                    "AND fra.attribute_id = @attributeId";

        var result = await connection.QueryFirstOrDefaultAsync<SubscriptionFareRuleCompositeOutput>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task<UserSubscriptionOutput?> GetUserSubscriptionByUserIdAsync(long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId);

        var query = "SELECT us.subscription_id, " +
                    "us.is_active, " +
                    "us.user_id," +
                    "asub.rule_id " +
                    "FROM subscriptions.user_subscriptions AS us " +
                    "INNER JOIN subscriptions.all_subscriptions AS asub " +
                    "ON us.subscription_id = asub.subscription_id " +
                    "WHERE us.user_id = @userId " +
                    "AND us.is_active";

        var result = await connection.QueryFirstOrDefaultAsync<UserSubscriptionOutput?>(query, parameters);

        return result;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}