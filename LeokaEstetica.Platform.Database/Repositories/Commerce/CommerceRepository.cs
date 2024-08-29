using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Base.Models.Input.Processing;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Orders;
using LeokaEstetica.Platform.Models.Entities.Commerce;
using LeokaEstetica.Platform.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Enum = LeokaEstetica.Platform.Models.Enums.Enum;

namespace LeokaEstetica.Platform.Database.Repositories.Commerce;

/// <summary>
/// Класс реализует методы репозитория коммерции.
/// </summary>
internal sealed class CommerceRepository : BaseRepository, ICommerceRepository
{
    private readonly PgContext _pgContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    /// <param name="connectionProvider">Провайдер к БД.</param>
    public CommerceRepository(PgContext pgContext,
        IConnectionProvider connectionProvider) : base(connectionProvider)
    {
        _pgContext = pgContext;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод создает заказ в БД.
    /// </summary>
    /// <param name="createPaymentOrderInput">Входная модель.</param>
    /// <returns>Данные заказа.</returns>
    public async Task<OrderOutput> CreateOrderAsync(CreatePaymentOrderInput createPaymentOrderInput)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@createdBy", createPaymentOrderInput.UserId);
        parameters.Add("@orderName", createPaymentOrderInput.Name);
        parameters.Add("@orderDetails", createPaymentOrderInput.Description);
        parameters.Add("@paymentId", createPaymentOrderInput.PaymentId);
        parameters.Add("@statusName", createPaymentOrderInput.PaymentStatusName);
        parameters.Add("@statusSysName", createPaymentOrderInput.PaymentStatusSysName);
        parameters.Add("@currency", new Enum(CurrencyTypeEnum.RUB));
        parameters.Add("@price", createPaymentOrderInput.Price);
        parameters.Add("@paymentMonth", createPaymentOrderInput.Price);
        parameters.Add("@totalPrice", createPaymentOrderInput.Price);
        
        //TODO: Передавать в этот метод тип заказа, не хардкодить.
        parameters.Add("@orderType", new Enum(OrderTypeEnum.FareRule));

        var query = "INSERT INTO commerce.orders (" +
                    "order_name, " +
                    "order_details, " +
                    "created_by, " +
                    "status_name, " +
                    "status_sys_name, " +
                    "price, " +
                    "total_price, " +
                    "currency, " +
                    "payment_month, " +
                    "payment_id, " +
                    "order_type) " +
                    "VALUES (" +
                    "@orderName, " +
                    "@orderDetails, " +
                    "@createdBy, " +
                    "@statusName, " +
                    "@statusSysName, " +
                    "@price, " +
                    "@totalPrice, " +
                    "@currency, " +
                    "@paymentMonth, " +
                    "@paymentId, " +
                    "@orderType) " +
                    "RETURNING order_id";

        var orderId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

        var orderParameters = new DynamicParameters();
        orderParameters.Add("@orderId", orderId);

        var orderQuery = "SELECT order_id, " +
                         "order_name, " +
                         "order_details, " +
                         "created_by, " +
                         "status_name, " +
                         "status_sys_name, " +
                         "price, " +
                         "total_price, " +
                         "currency, " +
                         "payment_month, " +
                         "payment_id, " +
                         "order_type " +
                         "FROM commerce.orders " +
                         "WHERE order_id = @orderId";
        
        var result = await connection.QuerySingleOrDefaultAsync<OrderOutput>(orderQuery, orderParameters);

        return result;
    }

    /// <summary>
    /// Метод получает скидку на услугу по ее типу и кол-ву месяцев.
    /// </summary>
    /// <param name="paymentMonth">Кол-во месяцев.</param>
    /// <param name="discountTypeEnum">Тип скидки на услугу</param>
    /// <returns>Скидка на услугу.</returns>
    public async Task<decimal> GetPercentDiscountAsync(short paymentMonth, DiscountTypeEnum discountTypeEnum)
    {
        var result = await _pgContext.DiscountRules
            .Where(d => d.Month == paymentMonth
                        && d.Type.Equals(discountTypeEnum.ToString()))
            .Select(d => Math.Round(d.Percent))
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод обновляет статус заказа.
    /// </summary>
    /// <param name="paymentStatusSysName">Системное название статуса заказа.</param>
    /// <param name="paymentStatusName">Русское название статуса заказа.</param>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="orderId">Id заказа в БД.</param>
    public async Task<bool> UpdateOrderStatusAsync(string paymentStatusSysName, string paymentStatusName,
        string paymentId, long orderId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var checkExistsParameters = new DynamicParameters();
        checkExistsParameters.Add("@paymentId", paymentId);
        checkExistsParameters.Add("@orderId", orderId);

        var checkExistsQuery = "SELECT EXISTS (" +
                               "SELECT order_id " +
                               "FROM commerce.orders " +
                               "WHERE order_id = @orderId " +
                               "AND payment_id = @paymentId)";

        var ifExists = await connection.ExecuteScalarAsync<bool>(checkExistsQuery, checkExistsParameters);

        if (!ifExists)
        {
            return false;
        }

        var updateOrderParameters = new DynamicParameters();
        updateOrderParameters.Add("@paymentStatusSysName", paymentStatusSysName);
        updateOrderParameters.Add("@paymentStatusName", paymentStatusName);
        updateOrderParameters.Add("@paymentId", paymentId);
        updateOrderParameters.Add("@orderId", orderId);

        var updateOrderQuery = "UPDATE commerce.orders " +
                               "SET status_name = @paymentStatusName, " +
                               "status_sys_name = @paymentStatusSysName " +
                               "WHERE order_id = @orderId";

        await connection.ExecuteAsync(updateOrderQuery, updateOrderParameters);

        return true;
    }

    /// <summary>
    /// Метод проставляет статус заказа подтвержден.
    /// </summary>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="paymentStatusSysName">Системное название статуса заказа.</param>
    /// <param name="paymentStatusName">Русское название статуса заказа.</param>
    public async Task SetStatusConfirmByPaymentIdAsync(string paymentId, string paymentStatusSysName,
        string paymentStatusName)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@paymentId", paymentId);
        parameters.Add("@paymentStatusSysName", paymentStatusSysName);
        parameters.Add("@paymentStatusName", paymentStatusName);

        var query = "UPDATE commerce.orders " +
                    "SET status_name = @paymentStatusName, " +
                    "status_sys_name = @paymentStatusSysName " +
                    "WHERE payment_id = @paymentId";

        await connection.ExecuteAsync(query, parameters);
    }

    /// <summary>
    /// Метод создает возврат в БД.
    /// Если paymentId = null, значит возврат создан вручную.
    /// Также признак IsManual - признак ручного создания возврата.
    /// </summary>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="price">Сумма возврата.</param>
    /// <param name="dateCreated">Дата создания возврата в ПС.</param>
    /// <param name="status">Статус возврата в ПС.</param>
    /// <param name="refundOrderId">Id возврата в ПС.</param>
    /// <param name="isManual">Признак ручного создания возврата.</param>
    /// <returns>Данные возврата.</returns>
    public async Task<RefundEntity> CreateRefundAsync(string paymentId, decimal price, DateTime dateCreated,
        string status, string refundOrderId, bool isManual)
    {
        var result = new RefundEntity
        {
            PaymentId = paymentId,
            Price = price,
            DateCreated = dateCreated,
            Status = status,
            RefundOrderId = refundOrderId,
            IsManual = isManual
        };

        await _pgContext.Refunds.AddAsync(result);
        await _pgContext.SaveChangesAsync();

        return result;
    }

    /// <summary>
    /// Метод обновляет статус возврата.
    /// </summary>
    /// <param name="refundStatusName">Русское название статуса возврата.</param>
    /// <param name="paymentId">Id платежа в ПС.</param>
    /// <param name="refundId">Id возврата в БД.</param>
    /// <param name="refundOrderId">Id возврата в ПС.</param>
    public async Task<bool> UpdateRefundStatusAsync(string refundStatusName, string paymentId, long refundId,
        string refundOrderId)
    {
        var refund = await _pgContext.Refunds
            .FirstOrDefaultAsync(r => r.PaymentId.Equals(paymentId)
                                      && r.RefundId == refundId
                                      && r.RefundOrderId.Equals(refundOrderId));
        
        if (refund is null)
        {
            return false;
        }
        
        refund.Status = refundStatusName;
        await _pgContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Метод создает чек возврата в БД.
    /// </summary>
    /// <param name="createReceiptOutput">Модель результата из ПС.</param>
    /// <returns>Данные чека.</returns>
    public async Task<ReceiptEntity> CreateReceiptRefundAsync(CreateReceiptOutput createReceiptOutput)
    {
        var receipt = new ReceiptEntity
        {
            DateCreated = createReceiptOutput.DateCreated,
            PaymentId = createReceiptOutput.PaymentId,
            ReceiptOrderId = createReceiptOutput.ReceiptId,
            Status = createReceiptOutput.Status,
            OrderId = createReceiptOutput.OrderId,
            Type = createReceiptOutput.Type
        };

        await _pgContext.Receipts.AddAsync(receipt);
        await _pgContext.SaveChangesAsync();

        return receipt;
    }

    /// <summary>
    /// Метод проверяет, существует ли уже такой возврат.
    /// </summary>
    /// <param name="orderId">Id заказа.</param>
    /// <returns>Признак результата проверки.</returns>
    public async Task<bool> IfExistsRefundAsync(string orderId)
    {
        var result = await _pgContext.Refunds.AnyAsync(r => r.RefundOrderId.Equals(orderId));

        return result;
    }

    /// <inheritdoc />
    public async Task<FeesOutput?> GetFeesByFareRuleIdAsync(int fareRuleId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@fareRuleId", fareRuleId);

        var query = "SELECT fees_id, " +
                    "fees_name, " +
                    "fees_sys_name, " +
                    "fees_price, " +
                    "fees_measure, " +
                    "fees_fare_rule_id, " +
                    "fees_is_active " +
                    "FROM commerce.fees " +
                    "WHERE fees_fare_rule_id = @fareRuleId " +
                    "AND fees_is_active";

        var result = await connection.QueryFirstOrDefaultAsync<FeesOutput?>(query, parameters);

        return result;
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}