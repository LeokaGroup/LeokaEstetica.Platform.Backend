using System.ComponentModel;

namespace LeokaEstetica.Platform.Core.Enums;

/// <summary>
/// Перечисление типов приглашений в проект.
/// </summary>
public enum ProjectInviteTypeEnum
{
    [Description("По ссылке")]
    Link = 1,
    
    [Description("По почте")]
    Email = 2,
    
    [Description("По номеру телефона")]
    PhoneNumber = 3,
    
    [Description("По логину")]
    Login = 4,
}