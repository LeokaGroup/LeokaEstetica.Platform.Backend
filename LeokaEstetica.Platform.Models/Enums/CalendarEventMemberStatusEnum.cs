using System.ComponentModel;

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление статусов участника события.
/// </summary>
public enum CalendarEventMemberStatusEnum
{
    /// <summary>
    /// Занят/занята.
    /// </summary>
    [Description("Занят/занята.")]
    Busy = 1,
    
    /// <summary>
    /// Возможно занят/занята.
    /// </summary>
    [Description("Возможно занят/занята.")]
    MayBeBusy = 2,
    
    /// <summary>
    /// Свободен/свободна.
    /// </summary>
    [Description("Свободен/свободна.")]
    Available = 3
}