using System.ComponentModel;

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов файлов тикета.
/// </summary>
public enum TicketFileTypeEnum
{
    [Description("Документ")]
    Document = 1,
    
    [Description("Изображение")]
    Image = 2
}