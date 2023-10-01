namespace LeokaEstetica.Platform.Models.Dto.Input.Ticket;

/// <summary>
/// Класс входной модели для создания предложения/пожелания.
/// </summary>
public class WisheOfferInput
{
    /// <summary>
    /// Почта пользователя, который оставил пожелание/предложение.
    /// </summary>
    public string ContactEmail { get; set; }

    /// <summary>
    /// Текст предложение/пожелания.
    /// </summary>
    public string WisheOfferText { get; set; }
}