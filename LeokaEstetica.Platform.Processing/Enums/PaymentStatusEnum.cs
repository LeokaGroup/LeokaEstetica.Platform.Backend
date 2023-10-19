using System.ComponentModel;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Core.Extensions;

namespace LeokaEstetica.Platform.Processing.Enums;

/// <summary>
/// Перечисление статусов платежа.
/// </summary>
public enum PaymentStatusEnum
{
    /// <summary>
    /// Статуса нет.
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Новый.
    /// </summary>
    [Description("Новый")]
    Pending = 1,
    
    /// <summary>
    /// Платеж проведен.
    /// </summary>
    [Description("Платеж проведен")]
    Settled = 2,
    
    /// <summary>
    /// Платеж авторизован.
    /// </summary>
    [Description("Платеж авторизован")]
    Authorized = 3,
    
    /// <summary>
    /// Платеж отменен.
    /// </summary>
    [Description("Платеж отменен")]
    Cancelled = 4,
    
    /// <summary>
    /// Платеж отклонен.
    /// </summary>
    [Description("Платеж отклонен")]
    Rejected = 5,
    
    /// <summary>
    /// Требуется дополнительное подтверждение.
    /// </summary>
    [Description("Требуется дополнительное подтверждение")]
    Confirmation = 6,
    
    /// <summary>
    /// Платеж оплачен, деньги авторизованы и ожидают списания.
    /// </summary>
    [Description("Оплачен")]
    WaitingForCapture = 7,
    
    /// <summary>
    /// Платеж успешно завершен.
    /// </summary>
    [Description("Завершен")]
    Succeeded = 8,
    
    /// <summary>
    /// Платеж отменен.
    /// </summary>
    [Description("Отменен")]
    Canceled = 9
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
        { PaymentStatusEnum.Confirmation.ToString(), PaymentStatusEnum.Confirmation },
        { PaymentStatusEnum.Canceled.ToString(), PaymentStatusEnum.Canceled },
        { PaymentStatusEnum.WaitingForCapture.ToString(), PaymentStatusEnum.WaitingForCapture },
        { PaymentStatusEnum.Succeeded.ToString(), PaymentStatusEnum.Succeeded }
    };

    /// <summary>
    /// Метод проверяет, есть ли такой статус в системе.
    /// </summary>
    /// <param name="statusName">Системное название статуса из ПС.</param>
    /// <returns>Если статус есть, вернет его, иначе будет по дефолту None.</returns>
    public static PaymentStatusEnum GetPaymentStatusBySysName(string statusName)
    {
        var result = _paymentStatuses.TryGet(statusName.ToPascalCase());

        if (result == PaymentStatusEnum.None)
        {
            result = _paymentStatuses.TryGet(statusName.ToPascalCaseFromSnakeCase());
        }
        
        return result;
    }
}