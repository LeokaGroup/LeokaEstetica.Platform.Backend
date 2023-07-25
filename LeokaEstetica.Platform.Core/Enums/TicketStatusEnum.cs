using System.ComponentModel;

namespace LeokaEstetica.Platform.Core.Enums;

/// <summary>
/// Перечисление статусов тикета.
/// </summary>
public enum TicketStatusEnum
{
    /// <summary>
    /// Тикет открыт.
    /// </summary>
    [Description("Открыт")]
    Opened = 1,
    
    /// <summary>
    /// Тикет закрыт.
    /// </summary>
    [Description("Закрыт")]
    Closed = 2
}