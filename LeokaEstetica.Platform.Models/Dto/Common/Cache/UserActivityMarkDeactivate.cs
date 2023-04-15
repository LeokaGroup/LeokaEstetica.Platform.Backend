using ProtoBuf;

namespace LeokaEstetica.Platform.Models.Dto.Input.User;

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
}