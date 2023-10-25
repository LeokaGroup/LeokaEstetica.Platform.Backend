using System.ComponentModel;
using LeokaEstetica.Platform.Core.Extensions;

namespace LeokaEstetica.Platform.Base.Enums;

/// <summary>
/// Перечисление статусов возвратов.
/// </summary>
public enum RefundStatusEnum
{
    /// <summary>
    /// Нет статуса.
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Успешный возврат.
    /// </summary>
    [Description("Возврат проведен успешно")]
    Success = 1,
    
    /// <summary>
    /// Возврат отклонен.
    /// </summary>
    [Description("Запрос на возврат отклонен")]
    Rejected = 2,
    
    /// <summary>
    /// Возврат ожидает обработки.
    /// </summary>
    [Description("Возврат ожидает обработки")]
    Pending = 3
}

/// <summary>
/// Класс статусов возврата.
/// </summary>
public static class RefundStatus
{
    private static readonly Dictionary<string, RefundStatusEnum> _refundStatuses = new()
    {
        { RefundStatusEnum.Success.ToString(), RefundStatusEnum.Success },
        { RefundStatusEnum.Rejected.ToString(), RefundStatusEnum.Rejected },
        { RefundStatusEnum.Pending.ToString(), RefundStatusEnum.Pending }
    };

    /// <summary>
    /// Метод проверяет, есть ли такой статус в системе.
    /// </summary>
    /// <param name="statusName">Системное название статуса из ПС.</param>
    /// <returns>Если статус есть, вернет его, иначе будет по дефолту None.</returns>
    public static RefundStatusEnum GetPaymentStatusBySysName(string statusName)
    {
        return _refundStatuses.TryGet(statusName);
    }
}