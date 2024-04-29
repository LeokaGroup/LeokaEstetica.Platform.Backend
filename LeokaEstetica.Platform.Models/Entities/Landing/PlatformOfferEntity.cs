namespace LeokaEstetica.Platform.Models.Entities.Landing;

/// <summary>
/// Класс сопоставляется с таблицей фона главного лендоса dbo.PlatformOffers.
/// </summary>
public class PlatformOfferEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int OfferId { get; set; }

    /// <summary>
    /// Заголовок.
    /// </summary>
    public string? OffeTitle { get; set; }

    /// <summary>
    /// Подзаголовок.
    /// </summary>
    public string? OfferSubTitle { get; set; }
}