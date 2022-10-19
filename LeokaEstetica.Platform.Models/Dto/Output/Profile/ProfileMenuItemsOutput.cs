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
    public List<Items> Items { get; set; }
    
    /// <summary>
    /// Системное название.
    /// </summary>
    public string SysName { get; set; }

    /// <summary>
    /// Путь.
    /// </summary>
    public string Url { get; set; }
}

public class Items
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