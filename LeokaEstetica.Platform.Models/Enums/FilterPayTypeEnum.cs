using System.ComponentModel;

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление фильтра оплат.
/// Используется при фильтрации по оплате.
/// </summary>
public enum FilterPayTypeEnum
{
    [Description("Отсутствует. Не ищем по этому значению.")]
    None = 1,
    
    [Description("Не имеет значения")]
    UnknownPay = 2,
    
    [Description("Есть оплата")]
    Pay = 3,
    
    [Description("Без оплаты")]
    NotPay = 4
}