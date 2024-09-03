using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Messaging.Abstractions.Mail;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Models.Entities.Commerce;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Processing.Models.Input;
using LeokaEstetica.Platform.RabbitMq.Abstractions;
using Microsoft.Extensions.Configuration;

namespace LeokaEstetica.Platform.Processing.Abstractions.Commerce;

/// <summary>
/// Абстракция сервиса заказов.
/// </summary>
public interface IOrdersService
{
    /// <summary>
    /// Метод получает список заказов пользователя.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список заказов пользователя.</returns>
    Task<IEnumerable<OrderEntity>> GetUserOrdersAsync(string account);

    /// <summary>
    /// Метод получает детали заказа по его Id.
    /// </summary>
    /// <param name="orderId">Id заказа.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Детали заказа.</returns>
    Task<OrderEntity> GetOrderDetailsAsync(long orderId, string account);

    /// <summary>
    /// Метод получает список транзакций по заказам пользователя.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список транзакций.</returns>
    Task<IEnumerable<HistoryEntity>> GetHistoryAsync(string account);

    /// <summary>
    /// TODO: Тут нужно передавать настройки кролика по аналогии как сделано в хабе модуля УП.
    /// TODO: Но только тут иначе нужно сделать такое (из конфигов получать по цепочке выше в самом верху, а строку урл получать в базовом контроллере попробовать, это тоже в самом верху цепочки).
    /// TODO: Передавать зависимости в объекте, их уже много тут.
    /// Метод создает результат созданного заказа. Также создает заказ в БД.
    /// </summary>
    /// <param name="createPaymentOrderAggregateInput">Агрегирующая модель заказа.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <param name="commerceRepository">Репозиторий коммерции.</param>
    /// <param name="rabbitMqService">Сервис кролика.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфига.</param>
    /// <param name="mailingsService">Сервис email.</param>
    /// <param name="vacancy">Данные вакансии.</param>
    /// <param name="orderType">Тип заказа.</param>
    /// <returns>Результирующая модель заказа.</returns>
    /// <exception cref="InvalidOperationException">Может бахнуть ошибку, если не прошла проверка статуса платежа в ПС.</exception>
    Task<ICreateOrderOutput> CreatePaymentOrderAsync(CreatePaymentOrderAggregateInput createPaymentOrderAggregateInput,
        IConfiguration configuration, ICommerceRepository commerceRepository, IRabbitMqService rabbitMqService,
        IGlobalConfigRepository globalConfigRepository, IMailingsService mailingsService, VacancyInput? vacancy,
        OrderTypeEnum orderType);
}