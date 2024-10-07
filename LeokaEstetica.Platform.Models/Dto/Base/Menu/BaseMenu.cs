namespace LeokaEstetica.Platform.Models.Dto.Base.Menu;

/// <summary>
/// Базовый класс меню.
/// </summary>
public class BaseMenu
{
    /// <summary>
    /// Элементы меню.
    /// </summary>
    public List<MenuItem>? Items { get; set; }
}

/// <summary>
/// Класс элемента меню.
/// </summary>
public class MenuItem
{
    /// <summary>
    /// Вложенные элементы меню.
    /// </summary>
    public List<MenuItem>? Items { get; set; }

    /// <summary>
    /// Название пункта.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Иконка пункта.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Признак блокировки пункта.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Признак видимости пункта.
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// Id Пункта (не числовой Id, а текстовый. Аналогия системного названия).
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Текст подсказки пункта.
    /// </summary>
    public string? Tooltip { get; set; }

    /// <summary>
    /// Положение подсказки тултипа.
    /// </summary>
    public string? TooltipPosition { get; set; }

    /// <summary>
    /// Позиция.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Команда. На бэке не используется, может использоваться на фронте для js-логики.
    /// </summary>
    public string? Command { get; set; }
}