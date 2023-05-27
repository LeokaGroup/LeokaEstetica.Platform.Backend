using System.ComponentModel;
using LeokaEstetica.Platform.Core.Extensions;

namespace LeokaEstetica.Platform.Processing.Enums;

/// <summary>
/// Перечисление статусов платежа.
/// </summary>
public enum PaymentStatusEnum
{
    None = 0,
    
    [Description("Новый")]
    Pending = 1,
    
    [Description("Платеж проведен")]
    Settled = 2,
    
    [Description("Платеж авторизован")]
    Authorized = 3,
    
    [Description("Платеж отменен")]
    Cancelled = 4,
    
    [Description("Платеж отклонен")]
    Rejected = 5,
    
    [Description("Требуется дополнительное подтверждение")]
    Confirmation = 6
}

/// <summary>
/// Класс статусов заказа.
/// </summary>
public static class PaymentStatus
{
    private static readonly Dictionary<string, PaymentStatusEnum> _paymentStatuses = new()
    {
        { PaymentStatusEnum.Pending.ToString(), PaymentStatusEnum.Pending },
        { PaymentStatusEnum.Settled.ToString(), PaymentStatusEnum.Settled },
        { PaymentStatusEnum.Authorized.ToString(), PaymentStatusEnum.Authorized },
        { PaymentStatusEnum.Cancelled.ToString(), PaymentStatusEnum.Cancelled },
        { PaymentStatusEnum.Rejected.ToString(), PaymentStatusEnum.Rejected },
        { PaymentStatusEnum.Confirmation.ToString(), PaymentStatusEnum.Confirmation }
    };

    /// <summary>
    /// Метод првоеряет, есть ли такой статус в системе.
    /// </summary>
    /// <param name="statusName">Системное название статуса из ПС.</param>
    /// <returns>Если статус есть, вернет его, иначе будет по дефолту None.</returns>
    public static PaymentStatusEnum GetPaymentStatusBySysName(string statusName)
    {
        return _paymentStatuses.TryGet(statusName);
    }

    public static bool IfExistPaymentStatusBySysName(string statusName)
    {
        return _paymentStatuses.ContainsKey(statusName);
    }
}