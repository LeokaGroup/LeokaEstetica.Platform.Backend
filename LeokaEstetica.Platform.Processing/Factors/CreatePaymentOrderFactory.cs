using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Extensions;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Base.Factors;
using LeokaEstetica.Platform.Base.Models.Input.Processing;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Messaging.Abstractions.Mail;
using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Models.Dto.Input.Base;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.YandexKassa;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.YandexKassa;
using LeokaEstetica.Platform.Processing.Consts;
using LeokaEstetica.Platform.Processing.Enums;
using LeokaEstetica.Platform.Processing.Models.Input;
using LeokaEstetica.Platform.Processing.Models.Output;
using LeokaEstetica.Platform.RabbitMq.Abstractions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Processing.Factors;

/// <summary>
/// Класс факторки создания результата заказа из ПС.
/// </summary>
public static class CreatePaymentOrderFactory
{
    /// <summary>
    /// TODO: Требуется серьезная доработка! Тут нужно передавать настройки кролика по аналогии как сделано в хабе модуля УП.
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
    /// <returns>Результирующая модель заказа.</returns>
    /// <exception cref="InvalidOperationException">Может бахнуть ошибку, если не прошла проверка статуса платежа в ПС.</exception>
    public static async Task<ICreateOrderOutput> CreatePaymentOrderResultAsync(
        CreatePaymentOrderAggregateInput createPaymentOrderAggregateInput, IConfiguration configuration,
        ICommerceRepository commerceRepository, IRabbitMqService rabbitMqService,
        IGlobalConfigRepository globalConfigRepository, IMailingsService mailingsService)
    {
        var paymentId = string.Empty;
        ICreateOrderOutput result = null;
        
        // using var httpClient = _httpClientFactory.CreateClient();
        // httpClient.SetHttpClientRequestAuthorizationHeader(token);

        using var httpClient = new HttpClient();

        if (createPaymentOrderAggregateInput.CreateOrderOutput is CreateOrderPayMasterOutput payMasterOrder)
        {
            paymentId = payMasterOrder.PaymentId;
        }

        else if (createPaymentOrderAggregateInput.CreateOrderOutput is CreateOrderYandexKassaOutput yandexOrder)
        {
            paymentId = yandexOrder.PaymentId;
            result = yandexOrder;
            
            httpClient.SetYandexKassaRequestAuthorizationHeader(configuration);
        }

        // Проверяем статус заказа в ПС.
        var responseCheckStatusOrder = await httpClient.GetStringAsync(
            string.Concat(ApiConsts.YandexKassa.CHECK_PAYMENT_STATUS, paymentId));

        // Если ошибка получения данных платежа.
        if (string.IsNullOrEmpty(responseCheckStatusOrder))
        {
            var ex = new InvalidOperationException(
                "Ошибка проверки статуса платежа в ПС. " +
                $"Данные платежа: {JsonConvert.SerializeObject(createPaymentOrderAggregateInput.CreateOrderInput)}");
            throw ex;
        }

        var createdOrder = await CreatePaymentOrderAsync(paymentId, createPaymentOrderAggregateInput.OrderCache,
            createPaymentOrderAggregateInput.CreateOrderInput, createPaymentOrderAggregateInput.UserId,
            responseCheckStatusOrder);

        // Создаем заказ в БД.
        var createdOrderResult = await commerceRepository.CreateOrderAsync(createdOrder);

        /// TODO: Засунуть все параметры в модель. Их слишком много уже тут.
        // Отправляем заказ в очередь для отслеживания его статуса.
        var orderEvent = OrderEventFactory.CreateOrderEvent(createdOrderResult.OrderId,
            createdOrderResult.StatusSysName, paymentId, createPaymentOrderAggregateInput.UserId,
            createPaymentOrderAggregateInput.PublicId, createPaymentOrderAggregateInput.OrderCache.Month,
            createdOrderResult.Price, createdOrderResult.Currency);

        // var configEnv = await httpClient.GetFromJsonAsync<ProxyConfigEnvironmentOutput>(string.Concat(apiUrl,
        //     GlobalConfigKeys.ProjectManagementProxyApi.PROJECT_MANAGEMENT_CONFIG_ENVIRONMENT_PROXY_API));
        //
        // if (configEnv is null)
        // {
        //     throw new InvalidOperationException("Не удалось получить среду окружения из конфига модуля УП.");
        // }
        //
        // var rabbitMqConfig = await httpClient.GetFromJsonAsync<ProxyConfigRabbitMqOutput>(string.Concat(apiUrl,
        //     GlobalConfigKeys.ProjectManagementProxyApi.PROJECT_MANAGEMENT_CONFIG_RABBITMQ_PROXY_API));
        //         
        // if (rabbitMqConfig is null)
        // {
        //     throw new InvalidOperationException("Не удалось получить настройки RabbitMQ из конфига модуля УП.");
        // }
            
        // var queueType = string.Empty.CreateQueueDeclareNameFactory(configEnv.Environment,
        //     QueueTypeEnum.ScrumMasterAiMessage);
        
        var queueType = string.Empty.CreateQueueDeclareNameFactory(configuration["Environment"],
            QueueTypeEnum.OrdersQueue);

        // var scrumMasterAiMessageEvent = ScrumMasterAiMessageEventFactory.CreateScrumMasterAiMessageEvent(message,
        //     token, userId);
        
        // await rabbitMqService.PublishAsync(orderEvent, queueType, rabbitMqConfig, configEnv);
        
        var isEnabledEmailNotifications = await globalConfigRepository.GetValueByKeyAsync<bool>(
            GlobalConfigKeys.EmailNotifications.EMAIL_NOTIFICATIONS_DISABLE_MODE_ENABLED);

        var sendMessage = $"Вы успешно оформили заказ: \"{createPaymentOrderAggregateInput.FareRuleName}\"";
        
        await mailingsService.SendNotificationCreatedOrderAsync(
            createPaymentOrderAggregateInput.Account, sendMessage, isEnabledEmailNotifications,
            createPaymentOrderAggregateInput.Month);

        return result;
    }

    /// <summary>
    /// Метод парсит результат для сохранения заказа в БД.
    /// </summary>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="orderCache">Заказ в кэше.</param>
    /// <param name="createOrderRequest">Модель запроса.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="responseCheckStatusOrder">Статус ответа из ПС.</param>
    /// <returns>Результирующая модель для сохранения в БД.</returns>
    private static async Task<CreatePaymentOrderInput> CreatePaymentOrderAsync(string paymentId,
        CreateOrderCache orderCache, ICreateOrderInput createOrderInput, long userId,
        string responseCheckStatusOrder)
    {
        var createOrder = JsonConvert.DeserializeObject<PaymentStatusOutput>(responseCheckStatusOrder);
        var result = new CreatePaymentOrderInput
        {
            PaymentId = paymentId,
            Name = orderCache.FareRuleName,
            UserId = userId,
            PaymentMonth = orderCache.Month,
            Currency = PaymentCurrencyEnum.RUB.ToString(),
            Created = DateTime.Parse(createOrder!.Created),
            PaymentStatusSysName = createOrder.OrderStatus,
            PaymentStatusName = PaymentStatusEnum.Pending.GetEnumDescription()
        };
        
        if (createOrderInput is CreateOrderPayMasterInput orderPayMasterInput)
        {
            result.Description = orderPayMasterInput.CreateOrderRequest.Invoice.Description;
            result.Price = orderPayMasterInput.CreateOrderRequest.Amount.Value;
        }

        else if (createOrderInput is CreateOrderYandexKassaInput orderYandexKassaInput)
        {
            result.Description = orderYandexKassaInput.CreateOrderRequest.Description;
            result.Price = orderYandexKassaInput.CreateOrderRequest.Amount.Value;
        }

        return await Task.FromResult(result);
    }
}