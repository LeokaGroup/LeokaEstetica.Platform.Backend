using System.ComponentModel;
using LeokaEstetica.Platform.Core.Extensions;

namespace LeokaEstetica.Platform.Processing.Enums;

/// <summary>
/// Перечисление статусов платежа.
/// </summary>
public enum PaymentStatusEnum
{
    [Description("Новый")]
    Pending = 1
}

public static class PaymentStatus
{
    public static readonly Dictionary<string, PaymentStatusEnum> PaymentStatuses = new()
    {
        { PaymentStatusEnum.Pending.ToString(), PaymentStatusEnum.Pending }
    };

    public static PaymentStatusEnum GetPaymentStatusByName(string statusName)
    {
        return PaymentStatuses.TryGet(statusName);
    }
}