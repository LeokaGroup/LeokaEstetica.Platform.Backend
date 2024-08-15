using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Base.Factors;
using LeokaEstetica.Platform.Base.Models.Input.Processing;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Orders;
using LeokaEstetica.Platform.Models.Entities.Commerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using IsolationLevel = System.Data.IsolationLevel;

namespace LeokaEstetica.Platform.Database.Repositories.Commerce;

/// <summary>
/// Класс реализует методы репозитория коммерции.
/// </summary>
internal sealed class CommerceRepository : BaseRepository, ICommerceRepository
{
    private readonly PgContext _pgContext;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    /// <param name="connectionProvider">Провайдер к БД.</param>
    public CommerceRepository(PgContext pgContext,
        IConfiguration configuration,
        IConnectionProvider connectionProvider) : base(connectionProvider)
    {
        _pgContext = pgContext;
        _configuration = configuration;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод создает заказ в БД.
    /// </summary>
    /// <param name="createPaymentOrderInput">Входная модель.</param>
    /// <returns>Данные заказа.</returns>
    public async Task<OrderOutput?> CreateOrderAsync(CreatePaymentOrderInput createPaymentOrderInput)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        using var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            var orderParameters = new DynamicParameters();
            orderParameters.Add("@createdBy", createPaymentOrderInput.UserId);
            orderParameters.Add("@paymentId", createPaymentOrderInput.PaymentId);
            orderParameters.Add("@orderName", createPaymentOrderInput.Name);
            orderParameters.Add("@orderDetails", createPaymentOrderInput.Description);
            orderParameters.Add("@price", createPaymentOrderInput.Price);
            orderParameters.Add("@currency", createPaymentOrderInput.Currency);
            orderParameters.Add("@createdAt", createPaymentOrderInput.Created);
            orderParameters.Add("@paymentMonth", createPaymentOrderInput.PaymentMonth);
            orderParameters.Add("@statusName", createPaymentOrderInput.PaymentStatusName);
            orderParameters.Add("@statusSysName", createPaymentOrderInput.PaymentStatusSysName);

            var orderQuery = "INSERT INTO commerce.orders (order_name, order_details, created_at, created_by," +
                             " status_name, status_sys_name, price, currency, payment_month) " +
                             "VALUES (@orderName, @orderDetails, @createdAt, @createdBy, @statusName," +
                             " @statusSysName, @price, @currency, @paymentMonth) " +
                             "RETURNING order_id";

            var orderId = await connection.ExecuteScalarAsync<long>(orderQuery, orderParameters);

            var resultParameters = new DynamicParameters();
            resultParameters.Add("@orderId", orderId);

            var resultQuery = "SELECT order_id, order_name, order_details, created_at, created_by, status_name," +
                              " status_sys_name, price, vat_rate, price_vat, discount, discount_price, total_price," +
                              " currency, payment_month, payment_id, order_type " +
                              "FROM commerce.orders " +
                              "WHERE order_id = @orderId";

            var result = await connection.QueryFirstOrDefaultAsync<OrderOutput?>(resultQuery, resultParameters);

            return result;
        }
        
        catch
        {
            transaction.Rollback();
            throw;
        }
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
        OrderEntity updateOrder;
        var isNew = false;
        PgContext pgContext = null;

        try
        {
            updateOrder = await _pgContext.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId 
                                          && o.PaymentId.Equals(paymentId));
        }
        
        // TODO: При dispose PgContext пересоздаем датаконтекст и пробуем снова.
        catch (ObjectDisposedException _)
        {
            pgContext = CreateNewPgContextFactory.CreateNewPgContext(_configuration);
            updateOrder = await pgContext.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId 
                                          && o.PaymentId.Equals(paymentId));
            isNew = true;
        }
        
        if (updateOrder is null)
        {
            return false;
        }

        updateOrder.StatusSysName = paymentStatusSysName;
        updateOrder.StatusName = paymentStatusName;

        if (isNew)
        {
            await pgContext.SaveChangesAsync();
        }

        else
        {
            await _pgContext.SaveChangesAsync();
        }

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
        OrderEntity order;
        var isNew = false;
        PgContext pgContext = null;
        
        try
        {
            order = await _pgContext.Orders.FirstOrDefaultAsync(o => o.PaymentId.Equals(paymentId));
        }
        
        // TODO: При dispose PgContext пересоздаем датаконтекст и пробуем снова.
        catch (ObjectDisposedException _)
        {
            pgContext = CreateNewPgContextFactory.CreateNewPgContext(_configuration);
            order = await pgContext.Orders.FirstOrDefaultAsync(o => o.PaymentId.Equals(paymentId));
            isNew = true;
        }

        if (order is null)
        {
            throw new InvalidOperationException($"Не удалось получить заказ по PaymentId: {paymentId}");
        }

        order.StatusName = paymentStatusName;
        order.StatusSysName = paymentStatusSysName;

        if (isNew)
        {
            await pgContext.SaveChangesAsync();
        }

        else
        {
            await _pgContext.SaveChangesAsync();   
        }
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

    #endregion

    #region Приватные методы.

    

    #endregion
}