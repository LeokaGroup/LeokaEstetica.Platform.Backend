using System.ComponentModel;

namespace LeokaEstetica.Platform.Access.Enums;

/// <summary>
/// Перечисление режимов.
/// </summary>
public enum ModeEnum
{
    [Description("Неизвестный режим")]
    None,
    
    [Description("Режим просмотра")]
    View,
    
    [Description("Режим изменения")]
    Edit
}