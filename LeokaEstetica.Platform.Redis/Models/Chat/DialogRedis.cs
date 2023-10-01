using ProtoBuf;

namespace LeokaEstetica.Platform.Redis.Models.Chat;

/// <summary>
/// Класс модели диалога для работы с кэшем Redis..
/// </summary>
[ProtoContract]
public class DialogRedis
{
    /// <summary>
    /// Id подключения пользователя.
    /// </summary>
    [ProtoMember(1)]
    public string ConnectionId { get; set; }

    /// <summary>
    /// Токен пользователя.
    /// </summary>
    [ProtoMember(2)]
    public string Token { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    [ProtoMember(3)]
    public long UserId { get; set; }
}