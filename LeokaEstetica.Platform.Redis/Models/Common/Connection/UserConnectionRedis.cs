using LeokaEstetica.Platform.Models.Enums;
using ProtoBuf;

namespace LeokaEstetica.Platform.Redis.Models.Common.Connection;

/// <summary>
/// Класс подключения пользователя для всех хабов для хранения в кэше.
/// </summary>
[ProtoContract]
public class UserConnectionRedis
{
    /// <summary>
    /// Id подключения.
    /// </summary>
    [ProtoMember(1)]
    public string? ConnectionId { get; set; }

    /// <summary>
    /// Модуль.
    /// </summary>
    [ProtoMember(2)]
    public UserConnectionModuleEnum Module { get; set; }

    /// <summary>
    /// Признак наличия в кэше.
    /// </summary>
    [ProtoMember(3)]
    public bool IsCacheExists { get; set; }
}