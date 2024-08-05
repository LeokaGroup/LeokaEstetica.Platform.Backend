using LeokaEstetica.Platform.Redis.Enums;

namespace LeokaEstetica.Platform.Redis.Models.Common.Connection;

/// <summary>
/// Класс подключения пользователя для всех хабов.
/// </summary>
public class UserConnection
{
    /// <summary>
    /// Id подключения.
    /// </summary>
    public string? ConnectionId { get; set; }

    /// <summary>
    /// Модуль.
    /// </summary>
    public UserConnectionModuleEnum Module { get; set; }

    /// <summary>
    /// Признак наличия в кэше.
    /// </summary>
    public bool IsCacheExists { get; set; }
}