namespace LeokaEstetica.Platform.Models.Dto.Output.Landing;

/// <summary>
/// Класс выходной модели блока предложений платформы.
/// </summary>
public class PlatformOfferOutput
{
    /// <summary>
    /// Заголовок.
    /// </summary>
    public string OffeTitle { get; set; }

    /// <summary>
    /// Подзаголовок.
    /// </summary>
    public string OfferSubTitle { get; set; }

    /// <summary>
    /// Список элементов предложений платформы.
    /// </summary>
    public IEnumerable<PlatformOfferItemsOutput> PlatformOfferItems { get; set; }
}