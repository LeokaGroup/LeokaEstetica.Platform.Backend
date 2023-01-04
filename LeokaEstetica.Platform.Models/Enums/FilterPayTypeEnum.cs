using System.ComponentModel;

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление фильтра оплат.
/// Используется при фильтрации по оплате.
/// </summary>
public enum FilterPayTypeEnum
{
    [Description("Не имеет значения")]
    Unknown = 1,
    
    [Description("Есть оплата")]
    Pay = 2,
    
    [Description("Без оплаты")]
    NotPay = 3
}