using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Access.Abstractions.AvailableLimits;
using LeokaEstetica.Platform.Access.Consts;
using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Access.Models.Output;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.AvailableLimits;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Access.Services.AvailableLimits;

/// <summary>
/// Класс реализует методы сервиса проверки лимитов.
/// </summary>
internal sealed class AvailableLimitsService : IAvailableLimitsService
{
    private readonly ILogger<AvailableLimitsService> _logger;
    private readonly IAvailableLimitsRepository _availableLimitsRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Сервис логера.</param>
    /// <param name="availableLimitsRepository">Репозиторий лимитов.</param>
    public AvailableLimitsService(ILogger<AvailableLimitsService> logger, 
        IAvailableLimitsRepository availableLimitsRepository)
    {
        _logger = logger;
        _availableLimitsRepository = availableLimitsRepository;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод проверяет, доступны ли пользователю для создания проекты в зависимости от подписки. 
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="fareRuleName">Название тарифа.</param>
    /// <returns>Признак доступости.</returns>
    public async Task<bool> CheckAvailableCreateProjectAsync(long userId, string fareRuleName)
    {
        try
        {
            // Получаем кол-во проектов пользователя.
            var userProjectsCount = await _availableLimitsRepository.CheckAvailableCreateProjectAsync(userId);

            // Проверяем кол-во в зависимости от подписки.
            // Если стартовый тариф.
            if (fareRuleName.Equals(FareRuleTypeEnum.Start.GetEnumDescription()))
            {
                return userProjectsCount <= AvailableLimitsConst.AVAILABLE_PROJECT_START_COUNT;
            }
            
            // Если базовый тариф.
            if (fareRuleName.Equals(FareRuleTypeEnum.Base.GetEnumDescription()))
            {
                return userProjectsCount <= AvailableLimitsConst.AVAILABLE_PROJECT_BASE_COUNT;
            }
            
            // Если бизнес тариф.
            if (fareRuleName.Equals(FareRuleTypeEnum.Business.GetEnumDescription()))
            {
                return userProjectsCount <= AvailableLimitsConst.AVAILABLE_PROJECT_BUSINESS_COUNT;
            }

            return true;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка проверки лимитов проектов пользователя. UserId был {userId}");
            throw;
        }
    }

    /// <summary>
    /// Метод проверяет, доступны ли пользователю для создания вакансии в зависимости от подписки. 
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="fareRuleName">Название тарифа.</param>
    /// <returns>Признак доступости.</returns>
    public async Task<bool> CheckAvailableCreateVacancyAsync(long userId, string fareRuleName)
    {
        try
        {
            // Получаем кол-во вакансий пользователя.
            var userVacanciesCount = await _availableLimitsRepository.CheckAvailableCreateVacancyAsync(userId);

            // Проверяем кол-во в зависимости от подписки.
            // Если стартовый тариф.
            if (fareRuleName.Equals(FareRuleTypeEnum.Start.GetEnumDescription()))
            {
                return userVacanciesCount < AvailableLimitsConst.AVAILABLE_VACANCY_START_COUNT;
            }
            
            // Если базовый тариф.
            if (fareRuleName.Equals(FareRuleTypeEnum.Base.GetEnumDescription()))
            {
                return userVacanciesCount < AvailableLimitsConst.AVAILABLE_VACANCY_BASE_COUNT;
            }
            
            // Если бизнес тариф.
            if (fareRuleName.Equals(FareRuleTypeEnum.Business.GetEnumDescription()))
            {
                return userVacanciesCount < AvailableLimitsConst.AVAILABLE_VACANCY_BUSINESS_COUNT;
            }

            return true;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка проверки лимитов вакансий пользователя. UserId был {userId}");
            throw;
        }
    }

    /// <summary>
    /// Метод проверяет, проходит ли понижающий тариф по лимитам при переходе на него с более дорогого тарифа.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="publicId">Публичный ключ тарифа, на который переходят.</param>
    /// <returns>Результирующая модель с результатами по лимитам.</returns>
    public async Task<ReductionSubscriptionLimitsOutput> CheckAvailableReductionSubscriptionAsync(long userId,
        Guid publicId, ISubscriptionRepository subscriptionRepository, IFareRuleRepository fareRuleRepository)
    {
        // Находим подписку.
        var subscription = await subscriptionRepository.GetUserSubscriptionAsync(userId);
        
        if (subscription is null)
        {
            throw new InvalidOperationException($"Не удалось получить подписку. UserId: {userId}");
        }

        // Находим подписку пользователя.
        var subscriptionId = subscription.SubscriptionId;
        var userSubscription = await subscriptionRepository.GetUserSubscriptionBySubscriptionIdAsync(
            subscriptionId, userId);

        if (userSubscription is null)
        {
            throw new InvalidOperationException("Не удалось получить подписку пользователя." +
                                                $"UserId: {userId}." +
                                                $"SubscriptionId: {subscriptionId}");
        }
        
        // Смотрим текущий тариф.
        var oldFare = await fareRuleRepository.GetByIdAsync(subscription.ObjectId);

        if (oldFare is null)
        {
            throw new InvalidOperationException($"Не удалось получить старый тариф пользователя. UserId: {userId}");
        }
        
        // Смотрим новый тариф.
        var newFare = await fareRuleRepository.GetByPublicIdAsync(publicId);
        
        if (newFare is null)
        {
            throw new InvalidOperationException($"Не удалось получить новый тариф пользователя. UserId: {userId}");
        }
        
        // Проверяем лимиты текущей и новой (которую хочет оформить) подписки пользователя.
        // Получаем кол-во проектов пользователя.
        var userProjectsCount = await _availableLimitsRepository.CheckAvailableCreateProjectAsync(userId);
        
        // Получаем кол-во вакансий пользователя.
        var userVacanciesCount = await _availableLimitsRepository.CheckAvailableCreateVacancyAsync(userId);

        var newFareName = newFare.Name;
        var result = new ReductionSubscriptionLimitsOutput
        {
            IsSuccessLimits = true,
            ReductionSubscriptionLimits = ReductionSubscriptionLimitsEnum.None.ToString()
        };
        
        // Если стартовый тариф.
        if (newFareName.Equals(FareRuleTypeEnum.Start.GetEnumDescription()))
        {
            // Если не проходит по кол-ву проектов.
            if (userProjectsCount > AvailableLimitsConst.AVAILABLE_PROJECT_START_COUNT)
            {
                result = await CreateReductionSubscriptionLimitsResult(ReductionSubscriptionLimitsEnum.Project,
                    userProjectsCount - AvailableLimitsConst.AVAILABLE_PROJECT_START_COUNT);
                
                return result;
            }
            
            // Если не проходит по кол-ву вакансий.
            if (userVacanciesCount > AvailableLimitsConst.AVAILABLE_VACANCY_START_COUNT)
            {
                result = await CreateReductionSubscriptionLimitsResult(ReductionSubscriptionLimitsEnum.Vacancy,
                    userProjectsCount - AvailableLimitsConst.AVAILABLE_VACANCY_START_COUNT);
                
                return result;
            }
        }
            
        // Если базовый тариф.
        if (newFareName.Equals(FareRuleTypeEnum.Base.GetEnumDescription()))
        {
            // Если не проходит по кол-ву проектов.
            if (userProjectsCount > AvailableLimitsConst.AVAILABLE_PROJECT_BASE_COUNT)
            {
                result = await CreateReductionSubscriptionLimitsResult(ReductionSubscriptionLimitsEnum.Project,
                    userProjectsCount - AvailableLimitsConst.AVAILABLE_PROJECT_BASE_COUNT);
                
                return result;
            }
            
            // Если не проходит по кол-ву вакансий.
            if (userVacanciesCount > AvailableLimitsConst.AVAILABLE_VACANCY_BASE_COUNT)
            {
                result = await CreateReductionSubscriptionLimitsResult(ReductionSubscriptionLimitsEnum.Vacancy,
                    userProjectsCount - AvailableLimitsConst.AVAILABLE_VACANCY_BASE_COUNT);
                
                return result;
            }
        }
            
        // Если бизнес тариф.
        if (newFareName.Equals(FareRuleTypeEnum.Business.GetEnumDescription()))
        {
            // Если не проходит по кол-ву проектов.
            if (userProjectsCount > AvailableLimitsConst.AVAILABLE_PROJECT_BUSINESS_COUNT)
            {
                result = await CreateReductionSubscriptionLimitsResult(ReductionSubscriptionLimitsEnum.Project,
                    userProjectsCount - AvailableLimitsConst.AVAILABLE_PROJECT_BUSINESS_COUNT);
                
                return result;
            }
            
            // Если не проходит по кол-ву вакансий.
            if (userVacanciesCount > AvailableLimitsConst.AVAILABLE_VACANCY_BUSINESS_COUNT)
            {
                result = await CreateReductionSubscriptionLimitsResult(ReductionSubscriptionLimitsEnum.Vacancy,
                    userProjectsCount - AvailableLimitsConst.AVAILABLE_VACANCY_BUSINESS_COUNT);
                
                return result;
            }
        }

        return result;
    }

    #region Приватные методы.

    /// <summary>
    /// Метод создает результат для выходной модели понижений лимитов.
    /// </summary>
    /// <param name="reductionSubscriptionLimitsType">Тип понижения подписки лимитов.</param>
    /// <param name="failCount">Кол-во, на которое нужно уменьшать, чтобы пройти по лимитам.</param>
    /// <returns>Результирующая модель.</returns>
    private async Task<ReductionSubscriptionLimitsOutput> CreateReductionSubscriptionLimitsResult(
        ReductionSubscriptionLimitsEnum reductionSubscriptionLimitsType, int failCount)
    {
        return await Task.FromResult(new ReductionSubscriptionLimitsOutput
        {
            IsSuccessLimits = false,
            ReductionSubscriptionLimits = reductionSubscriptionLimitsType.ToString(),
            FareLimitsCount = failCount
        });
    }

    #endregion

    #endregion
}