namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление статусов участника события.
/// </summary>
public enum CalendarEventMemberStatusEnum
{
    /// <summary>
    /// Занят/занята.
    /// </summary>
    Busy = 1,
    
    /// <summary>
    /// Возможно занят/занята.
    /// </summary>
    MayBeBusy = 2,
    
    /// <summary>
    /// Свободен/свободна.
    /// </summary>
    Available = 3
}