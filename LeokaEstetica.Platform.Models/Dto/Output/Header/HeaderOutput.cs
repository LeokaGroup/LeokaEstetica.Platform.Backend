namespace LeokaEstetica.Platform.Models.Dto.Output.Header;

/// <summary>
/// Класс выходной модели для хидера.
/// </summary>
public class HeaderOutput
{
    /// <summary>
    /// Название пункта меню хидера.
    /// </summary>
    public string MenuItemTitle { get; set; }

    /// <summary>
    /// Ссылка на роут.
    /// </summary>
    public string MenuItemUrl { get; set; }
}