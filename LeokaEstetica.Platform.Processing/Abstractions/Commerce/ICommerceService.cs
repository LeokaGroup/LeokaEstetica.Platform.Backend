using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Models.Dto.Output.FareRule;
using LeokaEstetica.Platform.Models.Dto.Output.Orders;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Processing.Builders.Order;
using LeokaEstetica.Platform.Processing.Enums;

namespace LeokaEstetica.Platform.Processing.Abstractions.Commerce;

/// <summary>
/// Абстракция сервиса коммерции.
/// </summary>
public interface ICommerceService
{
    /// <summary>
    /// Метод создает заказ в кэше или в кролике (зависит от тарифа, услуг).
    /// </summary>
    /// <param name="createOrderCache">Входная модель.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные заказа.</returns>
    Task<CreateOrderOutput> CreateOrderCacheOrRabbitMqAsync(CreateOrderInput createOrderCacheInput,
        string account);

    /// <summary>
    /// Метод получает услуги и сервисы заказа из кэша.
    /// </summary>
    /// <param name="orderBuilder">Билдер заказа.</param>
    /// <returns>Услуги и сервисы заказа.</returns>
    Task<CreateOrderCache> GetOrderProductsCacheAsync(BaseOrderBuilder orderBuilder);

    /// <summary>
    /// Метод вычисляет сумму с оставшихся дней подписки пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="orderId">Id заказа.</param>
    /// <returns>Сумма.</returns>
    Task<decimal> CalculatePriceSubscriptionFreeDaysAsync(long userId, long orderId);

    /// <summary>
    /// TODO: Выпилим, если будет не нужен.
    /// Метод вычисляет, есть ли остаток с прошлой подписки пользователя для учета ее как скидку при оформлении новой подписки.
    /// </summary>
    /// <param name="orderBuilder">Билдер заказа.</param>
    /// <returns>Сумма остатка, если она есть.</returns>
    Task<OrderFreeOutput> CheckFreePriceAsync(BaseOrderBuilder orderBuilder);

    /// <summary>
    /// Метод проверяет заполнение анкеты пользователя.
    /// Если не заполнена, то нельзя оформить заказ.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Признак результата проверки.</returns>
    Task<bool> IsProfileEmptyAsync(string account);
    
    /// <summary>
    /// Метод создает заказ.
    /// </summary>
    /// <param name="orderBuilder">Билдер заказа.</param>
    /// <returns>Данные платежа.</returns>
    Task<ICreateOrderOutput> CreateOrderAsync(BaseOrderBuilder orderBuilder);
    
    /// <summary>
    /// Метод проверяет статус платежа в ПС.
    /// </summary>
    /// <param name="orderBuilder">Билдер заказа.</param>
    /// <returns>Статус платежа.</returns>
    Task<PaymentStatusEnum> CheckOrderStatusAsync(BaseOrderBuilder orderBuilder);

    /// <summary>
    /// Метод подтвержадет платеж в ПС. После этого спишутся ДС.
    /// </summary>
    /// <param name="orderBuilder">Билдер заказа.</param>
    Task ConfirmPaymentAsync(BaseOrderBuilder orderBuilder);

    /// <summary>
    /// Метод вычисляет цену тарифа исходя из параметров.
    /// </summary>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="selectedMonth">Кол-во месяцев подписки.</param>
    /// <param name="employeeCount">Кол-во сотрудников в организации.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Выходная модель.</returns>
    Task<CalculateFareRulePriceOutput> CalculateFareRulePriceAsync(Guid publicId, int selectedMonth,
        int employeeCount, string account);

    /// <summary>
    /// Метод вычисляет цену за публикацию вакансии в соответствии с тарифом пользователя.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Выходная модель.</returns>
    Task<CalculatePostVacancyPriceOutput> CalculatePricePostVacancyAsync(string account);
}