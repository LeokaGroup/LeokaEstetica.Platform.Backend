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
    Pending = 1
}

/// <summary>
/// Класс статусов заказа.
/// </summary>
public static class PaymentStatus
{
    private static readonly Dictionary<string, PaymentStatusEnum> _paymentStatuses = new()
    {
        { PaymentStatusEnum.Pending.ToString(), PaymentStatusEnum.Pending }
    };

    public static PaymentStatusEnum GetPaymentStatusBySysName(string statusName)
    {
        return _paymentStatuses.TryGet(statusName);
    }

    public static bool IfExistPaymentStatusBySysName(string statusName)
    {
        return _paymentStatuses.ContainsKey(statusName);
    }
}