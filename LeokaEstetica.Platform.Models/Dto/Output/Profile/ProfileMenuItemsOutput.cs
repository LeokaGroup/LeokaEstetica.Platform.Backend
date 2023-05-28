namespace LeokaEstetica.Platform.Models.Dto.Output.Profile;

/// <summary>
/// Класс выходной модели для списков меню профиля пользователя.
/// </summary>
public class ProfileMenuItemsOutput
{
    /// <summary>
    /// Название пункта.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Список элементов.
    /// </summary>
    public List<ProfileItems> Items { get; set; } = new();
    
    /// <summary>
    /// Системное название.
    /// </summary>
    public string SysName { get; set; }

    /// <summary>
    /// Путь.
    /// </summary>
    public string Url { get; set; }
}

/// <summary>
/// Класс вложенных элементов списка меню.
/// </summary>
public class ProfileItems
{
    /// <summary>
    /// Название.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Системное название.
    /// </summary>
    public string SysName { get; set; }

    /// <summary>
    /// Путь.
    /// </summary>
    public string Url { get; set; }
}