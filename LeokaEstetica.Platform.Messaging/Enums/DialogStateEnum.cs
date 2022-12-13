using System.ComponentModel;

namespace LeokaEstetica.Platform.Messaging.Enums;

/// <summary>
/// Перечисление состояний диалога.
/// None - Открыть пустую область, без диалогов.
/// Empty - диалог новый, открыть пустой диалог.
/// Open - Диалог открыт.
/// </summary>
public enum DialogStateEnum
{
    [Description("Диалог закрыт.")]
    None,
    
    [Description("Диалог пуст.")]
    Empty,
    
    [Description("Диалог открыт.")]
    Open
}