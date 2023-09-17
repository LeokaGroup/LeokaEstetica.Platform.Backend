namespace LeokaEstetica.Platform.Models.Dto.Output.Communication;

/// <summary>
/// Класс выходной модели публичной оферты.
/// </summary>
public class PublicOfferOutput
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