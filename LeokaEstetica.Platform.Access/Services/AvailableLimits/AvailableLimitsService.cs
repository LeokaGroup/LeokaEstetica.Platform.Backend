using LeokaEstetica.Platform.Access.Abstractions.AvailableLimits;
using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.AvailableLimits;
using LeokaEstetica.Platform.Logs.Abstractions;

namespace LeokaEstetica.Platform.Access.Services.AvailableLimits;

/// <summary>
/// Класс реализует методы сервиса проверки лимитов.
/// </summary>
public class AvailableLimitsService : IAvailableLimitsService
{
    private readonly ILogService _logService;
    private readonly IAvailableLimitsRepository _availableLimitsRepository;
    private const int AVAILABLE_PROJECT_START_COUNT = 4; // Кол-во у тарифа старта.
    private const int AVAILABLE_PROJECT_BASE_COUNT = 10; // Кол-во у тарифа базовый.
    private const int AVAILABLE_PROJECT_BUSINESS_COUNT = 35; // Кол-во у тарифа бизнес.

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logService">Сервис логера.</param>
    public AvailableLimitsService(ILogService logService, 
        IAvailableLimitsRepository availableLimitsRepository)
    {
        _logService = logService;
        _availableLimitsRepository = availableLimitsRepository;
    }

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
                return userProjectsCount < AVAILABLE_PROJECT_START_COUNT;
            }
            
            // Если базовый тариф.
            if (fareRuleName.Equals(FareRuleTypeEnum.Base.GetEnumDescription()))
            {
                return userProjectsCount < AVAILABLE_PROJECT_BASE_COUNT;
            }
            
            // Если бизнес тариф.
            if (fareRuleName.Equals(FareRuleTypeEnum.Business.GetEnumDescription()))
            {
                return userProjectsCount < AVAILABLE_PROJECT_BUSINESS_COUNT;
            }

            return true;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex, $"Ошибка проверки лимитов проектов пользователя. UserId был {userId}");
            throw;
        }
    }
}