namespace LeokaEstetica.Platform.Models.Dto.Output.Profile;

/// <summary>
/// Класс выходной модели с результатами для меню профиля пользователя.
/// </summary>
public class ProfileMenuItemsResultOutput
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