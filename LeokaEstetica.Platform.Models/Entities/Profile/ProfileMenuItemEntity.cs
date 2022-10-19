using LeokaEstetica.Platform.Models.Dto.Output.Profile;

namespace LeokaEstetica.Platform.Models.Entities.Profile;

/// <summary>
/// Класс сопоставляется с таблицей меню профиля пользователя Profile.ProfileMenuItems.
/// </summary>
public class ProfileMenuItemEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int ProfileMenuItemId { get; set; }

    /// <summary>
    /// Элементы меню профиля.
    /// </summary>
    public List<ProfileMenuItemsOutput> ProfileMenuItems { get; set; }
}