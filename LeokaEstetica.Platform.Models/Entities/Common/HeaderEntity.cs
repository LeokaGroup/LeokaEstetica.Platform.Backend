namespace LeokaEstetica.Platform.Models.Entities.Common;

/// <summary>
/// Класс сопоставляется с таблицей dbo.Header.
/// </summary>
public class HeaderEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int HeaderId { get; set; }

    /// <summary>
    /// Название пункта меню хидера.
    /// </summary>
    public string HeaderMenuItemTitle { get; set; }

    /// <summary>
    /// Ссылка на роут.
    /// </summary>
    public string HeaderMenuItemUrl { get; set; }

    /// <summary>
    /// Позиция в списке.
    /// </summary>
    public int Position { get; set; }
}