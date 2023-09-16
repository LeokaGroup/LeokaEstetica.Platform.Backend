namespace LeokaEstetica.Platform.Models.Entities.Communication;

/// <summary>
/// Класс сопоставляется с таблицей публичной оферты Communications.PublicOffer.
/// </summary>
public class PublicOfferEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public short OfferId { get; set; }

    /// <summary>
    /// Название пункта оферты.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Описание.
    /// </summary>
    public string Description { get; set; }
}