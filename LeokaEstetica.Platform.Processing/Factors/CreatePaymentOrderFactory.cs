using LeokaEstetica.Platform.Base.Enums;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Base.Helpers;
using LeokaEstetica.Platform.Base.Models.Input.Processing;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Models.Dto.Input.Base;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.YandexKassa;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.YandexKassa;
using LeokaEstetica.Platform.Processing.Consts;
using LeokaEstetica.Platform.Processing.Enums;
using LeokaEstetica.Platform.Processing.Models;
using LeokaEstetica.Platform.Processing.Models.Input;
using LeokaEstetica.Platform.Processing.Models.Output;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Processing.Factors;

/// <summary>
/// Класс факторки создания результата заказа из ПС.
/// </summary>
public static class CreatePaymentOrderFactory
{
    /// <summary>
    /// Метод создает результат созданного заказа. Также создает заказ в БД.
    /// </summary>
    /// <param name="createPaymentOrderAggregateInput">Агрегирующая модель заказа.</param>
    /// <param name="createPaymentOrderReference">Необходимые зависимости.</param>
    /// <returns>Результирующая модель заказа.</returns>
    /// <exception cref="InvalidOperationException">Может бахнуть ошибку, если не прошла проверка статуса платежа в ПС.</exception>
    public static async Task<ICreateOrderOutput> CreatePaymentOrderResultAsync(
        CreatePaymentOrderAggregateInput createPaymentOrderAggregateInput,
        IPaymentOrderReference createPaymentOrderReference)
    {
        var paymentId = string.Empty;
        ICreateOrderOutput result = null;

        if (createPaymentOrderAggregateInput.CreateOrderOutput is CreateOrderPayMasterOutput payMasterOrder)
        {
            paymentId = payMasterOrder.PaymentId;
        }

        else if (createPaymentOrderAggregateInput.CreateOrderOutput is CreateOrderYandexKassaOutput yandexOrder)
        {
            paymentId = yandexOrder.PaymentId;
        }

        // Проверяем статус заказа в ПС.
        var responseCheckStatusOrder = await createPaymentOrderAggregateInput.HttpClient.GetStringAsync(
            string.Concat(ApiConsts.PayMaster.CHECK_PAYMENT_STATUS, paymentId));

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
        var createdOrderResult = await createPaymentOrderReference.CommerceRepository.CreateOrderAsync(createdOrder);

        // Отправляем заказ в очередь для отслеживания его статуса.
        var orderEvent = OrderEventFactory.CreateOrderEvent(createdOrderResult.OrderId,
            createdOrderResult.StatusSysName, paymentId, createPaymentOrderAggregateInput.UserId,
            createPaymentOrderAggregateInput.PublicId, createPaymentOrderAggregateInput.OrderCache.Month);

        var queueType = string.Empty.CreateQueueDeclareNameFactory(createPaymentOrderReference.Configuration,
            QueueTypeEnum.OrdersQueue);
        await createPaymentOrderReference.RabbitMqService.PublishAsync(orderEvent, queueType);

        if (createPaymentOrderAggregateInput.CreateOrderOutput is CreateOrderPayMasterOutput payMasterOrderResult)
        {
            result = new CreateOrderPayMasterOutput
            {
                PaymentId = payMasterOrderResult.PaymentId,
                Url = payMasterOrderResult.Url
            };
        }

        else if (createPaymentOrderAggregateInput.CreateOrderOutput is CreateOrderYandexKassaOutput yandexOrderResult)
        {
            result = new CreateOrderYandexKassaOutput
            {
                PaymentId = yandexOrderResult.PaymentId,
                Url = yandexOrderResult.Url
            };
        }

        var isEnabledEmailNotifications = await createPaymentOrderReference.GlobalConfigRepository
            .GetValueByKeyAsync<bool>(GlobalConfigKeys.EmailNotifications.EMAIL_NOTIFICATIONS_DISABLE_MODE_ENABLED);

        var sendMessage = $"Вы успешно оформили заказ: \"{createPaymentOrderAggregateInput.FareRuleName}\"";
        await createPaymentOrderReference.MailingsService.SendNotificationCreatedOrderAsync(
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