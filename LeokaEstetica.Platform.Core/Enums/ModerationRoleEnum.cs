using System.ComponentModel;

namespace LeokaEstetica.Platform.Core.Enums;

/// <summary>
/// Перечисление роле модерации.
/// </summary>
public enum ModerationRoleEnum
{
    [Description("Администратор")]
    Administrator = 1,
    
    [Description("Модератор")]
    Moderator = 2
}