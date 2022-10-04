namespace LeokaEstetica.Platform.Models.Dto.Output.Landing;

/// <summary>
/// Класс выходной модели списка элементов блока предложений платформы. 
/// </summary>
public class PlatformOfferItemsOutput
{
    /// <summary>
    /// Текст элемента.
    /// </summary>
    public string ItemText { get; set; }

    /// <summary>
    /// Иконка элемента.
    /// </summary>
    public string ItemIcon { get; set; }
}