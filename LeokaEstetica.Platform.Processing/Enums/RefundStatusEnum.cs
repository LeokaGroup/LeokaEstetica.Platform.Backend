using System.ComponentModel;

namespace LeokaEstetica.Platform.Processing.Enums;

/// <summary>
/// Перечисление статусов возвратов.
/// </summary>
public enum RefundStatusEnum
{
    None = 0,
    
    [Description("Возврат проведен успешно")]
    Success = 1,
    
    [Description("Запрос на возврат отклонен")]
    Rejected = 2,
    
    [Description("Возврат выполняется")]
    Pending = 3
}