namespace LeokaEstetica.Platform.Models.Entities.Communication;

/// <summary>
/// Класс сопоставляется с таблицей пожеланий/предложений Communications.WishesOffers.
/// </summary>
public class WisheOfferEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long WisheOfferId { get; set; }

    /// <summary>
    /// Почта пользователя, который оставил пожелание/предложение.
    /// </summary>
    public string ContactEmail { get; set; }

    /// <summary>
    /// Текст предложение/пожелания.
    /// </summary>
    public string WisheOfferText { get; set; }

    /// <summary>
    /// Дата создания.
    /// </summary>
    public DateTime DateCreated { get; set; }
}