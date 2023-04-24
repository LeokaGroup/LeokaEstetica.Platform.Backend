using ProtoBuf;

namespace LeokaEstetica.Platform.Models.Dto.Common.Cache;

/// <summary>
/// Класс модели для хранения в кэше пользователей предупрежденных об удалении аккаунтов.
/// </summary>
[Serializable]
[ProtoContract]
public class UserActivityMarkDeactivate
{
    /// <summary>
    /// Id пользователя.
    /// </summary>
    [ProtoMember(1)]
    public long UserId { get; set; }

    /// <summary>
    /// Почта пользователя.
    /// </summary>
    [ProtoMember(2)]
    public string Email { get; set; }
}