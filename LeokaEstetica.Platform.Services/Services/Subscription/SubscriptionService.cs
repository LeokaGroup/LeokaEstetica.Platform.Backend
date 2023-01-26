using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Subscription;
using LeokaEstetica.Platform.Models.Entities.Subscription;
using LeokaEstetica.Platform.Services.Abstractions.Subscription;
using LeokaEstetica.Platform.Services.Builders;

namespace LeokaEstetica.Platform.Services.Services.Subscription;

/// <summary>
/// Класс реализует методы сервиса подписок.
/// </summary>
public class SubscriptionService : ISubscriptionService
{
    private readonly ILogService _logService;
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    
    public SubscriptionService(ILogService logService, 
        IUserRepository userRepository, 
        ISubscriptionRepository subscriptionRepository)
    {
        _logService = logService;
        _userRepository = userRepository;
        _subscriptionRepository = subscriptionRepository;
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
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод проставляет выделенные подписки пользователю.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="subscriptions">Список подписок до выделения.</param>
    /// <returns>Список подписок, но с выделенной подпиской, которую оформил пользователь либо не выделяем.</returns>
    public async Task<List<SubscriptionOutput>> FillSubscriptionsAsync(string account, List<SubscriptionOutput> subscriptions)
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
            if (!userSubscriptions.Any())
            {
                return subscriptions;
            }

            // Проставляем выделение подпискам.
            FillSubscriptionsBuilder.Fill(ref subscriptions, userSubscriptions);

            return subscriptions;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }
}