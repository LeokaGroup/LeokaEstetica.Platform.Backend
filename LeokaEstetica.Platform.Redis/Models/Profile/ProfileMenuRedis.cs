using ProtoBuf;

namespace LeokaEstetica.Platform.Redis.Models.Profile;

/// <summary>
/// Класс модели для хранения профиля пользователя для работы с кэшем Redis.
/// </summary>
[ProtoContract]
public class ProfileMenuRedis
{
    /// <summary>
    /// Элементы меню профиля.
    /// </summary>
    [ProtoMember(1)]
    public List<ProfileMenuItemsRedis> ProfileMenuItems { get; set; }
}