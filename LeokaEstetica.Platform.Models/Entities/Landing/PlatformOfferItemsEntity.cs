namespace LeokaEstetica.Platform.Models.Entities.Landing;

/// <summary>
/// Класс сопоставляется с таблицей элементов фона главного лендоса dbo.PlatformOffersItems.
/// </summary>
public class PlatformOfferItemsEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int ItemId { get; set; }

    /// <summary>
    /// Текст элемента.
    /// </summary>
    public string ItemText { get; set; }

    /// <summary>
    /// Иконка элемента.
    /// </summary>
    public string ItemIcon { get; set; }

    /// <summary>
    /// Позиция элемента в списке.
    /// </summary>
    public int Position { get; set; }
    
    /// <summary>
    /// Признак функционала, который будет позже.
    /// </summary>
    public bool IsLater { get; set; }
}