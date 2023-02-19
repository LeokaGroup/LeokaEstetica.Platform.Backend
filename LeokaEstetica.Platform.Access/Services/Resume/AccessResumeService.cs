using LeokaEstetica.Platform.Access.Abstractions.Resume;
using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Resume;

namespace LeokaEstetica.Platform.Access.Services.Resume;

/// <summary>
/// Класс реализует методы сервиса доступа к базе резюме.
/// </summary>
public class AccessResumeService : IAccessResumeService
{
    private readonly ILogService _logService;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;
    
    public AccessResumeService(ILogService logService, 
        ISubscriptionRepository subscriptionRepository, 
        IUserRepository userRepository)
    {
        _logService = logService;
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Метод проверяет доступ пользователя.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Число, определяющее уровень доступа.</returns>
    public async Task<AcessResumeOutput> CheckAvailableResumesAsync(string account)
    {
        try
        {
            var result = new AcessResumeOutput();
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            var userSubscription = await _subscriptionRepository.GetUserSubscriptionAsync(userId);

            // Не даем доступ.
            if (userSubscription is null)
            {
                result.Access = (int)FareRuleTypeEnum.NotAvailable;

                return result;
            }

            if (userSubscription.ObjectId < 1)
            {
                result.Access = (int)FareRuleTypeEnum.NotAvailable;

                return result;
            }
            
            if (userSubscription.ObjectId == 2)
            {
                result.Access = (int)FareRuleTypeEnum.Base;
            }
            
            if (userSubscription.ObjectId == 3)
            {
                result.Access = (int)FareRuleTypeEnum.Business;
            }
            
            if (userSubscription.ObjectId == 4)
            {
                result.Access = (int)FareRuleTypeEnum.Professional;
            }

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex, "Ошибка проверки доступа пользователя.");
            throw;
        }
    }
}