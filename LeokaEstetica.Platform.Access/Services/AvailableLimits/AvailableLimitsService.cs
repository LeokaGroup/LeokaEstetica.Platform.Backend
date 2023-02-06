using LeokaEstetica.Platform.Access.Abstractions.AvailableLimits;
using LeokaEstetica.Platform.Database.Abstractions.AvailableLimits;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Logs.Abstractions;

namespace LeokaEstetica.Platform.Access.Services.AvailableLimits;

/// <summary>
/// Класс реализует методы сервиса проверки лимитов.
/// </summary>
public class AvailableLimitsService : IAvailableLimitsService
{
    private readonly ILogService _logService;
    private readonly IAvailableLimitsRepository _availableLimitsRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logService">Сервис логера.</param>
    public AvailableLimitsService(ILogService logService, 
        IAvailableLimitsRepository availableLimitsRepository, 
        ISubscriptionRepository subscriptionRepository)
    {
        _logService = logService;
        _availableLimitsRepository = availableLimitsRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    /// <summary>
    /// Метод проверяет, доступны ли пользователю для создания проекты в зависимости от подписки. 
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Признак доступости.</returns>
    public async Task<bool> CheckAvailableCreateProjectAsync(long userId)
    {
        try
        {
            // Получаем подписку пользователя.
            var userSubscription = await _subscriptionRepository.GetUserSubscriptionAsync(userId);
            
            // Получаем кол-во проектов пользователя.
            var userProjectsCount = await _availableLimitsRepository.CheckAvailableCreateProjectAsync(userId);
            
            // Проверяем кол-во в зависимости от подписки.

            return true;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex, $"Ошибка проверки лимитов проектов пользователя. UserId был {userId}");
            throw;
        }
    }
}