using System.Text;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Core.Structs;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce;
using LeokaEstetica.Platform.Processing.Abstractions.Commerce;
using LeokaEstetica.Platform.Redis.Abstractions.Commerce;
using LeokaEstetica.Platform.Redis.Consts;

namespace LeokaEstetica.Platform.Processing.Services.Commerce;

/// <summary>
/// Класс реализует методы сервиса коммерции.
/// </summary>
public class CommerceService : ICommerceService
{
    private readonly ICommerceRedisService _commerceRedisService;
    private readonly ILogService _logService;
    private readonly IUserRepository _userRepository;
    private readonly IFareRuleRepository _fareRuleRepository;
    private readonly ICommerceRepository _commerceRepository;
    
    /// <summary>
    /// Конструктор.
    /// <param name="commerceRedisService">Сервис кэша коммерции.</param>
    /// <param name="logService">Сервис логера.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="fareRuleRepository">Репозиторий правил тарифов.</param>
    /// <param name="fareRuleRepository">Репозиторий коммерции.</param>
    /// </summary>
    public CommerceService(ICommerceRedisService commerceRedisService, 
        ILogService logService, 
        IUserRepository userRepository, 
        IFareRuleRepository fareRuleRepository, 
        ICommerceRepository commerceRepository)
    {
        _commerceRedisService = commerceRedisService;
        _logService = logService;
        _userRepository = userRepository;
        _fareRuleRepository = fareRuleRepository;
        _commerceRepository = commerceRepository;
    }

    #region Публичные методы.

    // <summary>
    /// Метод создает заказ в кэше.
    /// </summary>
    /// <param name="createOrderCache">Модель заказа для хранения в кэше.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные заказа добавленного в кэш.</returns>
    public async Task<CreateOrderCache> CreateOrderCacheAsync(CreateOrderCacheInput createOrderCacheInput,
        string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            // Сохраняем заказ в кэш сроком на 2 часа.
            var publicId = createOrderCacheInput.PublicId;
            var key = CreateOrderCacheKey(userId, publicId);
            var orderToCache = await CreateOrderCacheResult(publicId, createOrderCacheInput.PaymentMonth);
            
            var result = await _commerceRedisService.CreateOrderCacheAsync(key, orderToCache);

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод создает ключ для добавления заказа в кэш.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="publicId">Публичный код тарифа.</param>
    /// <returns>Ключ для добавления заказа в кэш.</returns>
    private string CreateOrderCacheKey(long userId, Guid publicId)
    {
        var builder = new StringBuilder();
        builder.Append(CacheKeysConsts.ORDER_CACHE);
        builder.Append(userId);
        builder.Append('_');
        builder.Append(publicId);

        return builder.ToString();
    }

    /// <summary>
    /// Метод создает модель заказа для кэша.
    /// </summary>
    /// <param name="publicId">Публичный код тарифа.</param>
    /// <param name="paymentMonth">Кол-во месяцев подписки.</param>
    /// <returns>Результирующая модель.</returns>
    private async Task<CreateOrderCache> CreateOrderCacheResult(Guid publicId, short paymentMonth)
    {
        var rule = await _fareRuleRepository.GetByPublicIdAsync(publicId);

        if (rule is null)
        {
            var ex = new InvalidOperationException($"Не удалось найти правило тарифа. PublicId: {publicId}");
            throw ex;
        }

        var discount = await GetPercentDiscountAsync(paymentMonth, DiscountTypeEnum.Service);

        var result = new CreateOrderCache
        {
            RuleId = rule.RuleId,
            Month = paymentMonth,
            Percent = discount.Percent,
            Price = discount.Price
        };

        return result;
    }

    /// <summary>
    /// Метод получает скидку на услугу по ее типу и кол-ву месяцев.
    /// </summary>
    /// <param name="paymentMonth">Кол-во месяцев.</param>
    /// <param name="discountTypeEnum">Тип скидки на услугу</param>
    /// <returns>Скидка на услугу.</returns>
    private async Task<DiscountStruct> GetPercentDiscountAsync(short paymentMonth, DiscountTypeEnum discountTypeEnum)
    {
        var result = await _commerceRepository.GetPercentDiscountAsync(paymentMonth, discountTypeEnum);

        return result;
    }

    #endregion
}