using System.ComponentModel;

namespace LeokaEstetica.Platform.Core.Enums;

/// <summary>
/// Перечисление статусов замечаний.
/// </summary>
public enum RemarkStatusEnum
{
    [Description("Ожидает исправления")]
    AwaitingCorrection = 1,
    
    [Description("Исправлено")]
    Fixed = 2,
    
    [Description("Не отправлено")]
    NotAssigned = 3,
    
    [Obsolete("Не используется. В будущем возможно удалим.")]
    [Description("Повторно отправлено")]
    AgainAssigned = 4,
    
    [Description("Ожидает проверки")]
    Review = 5
}