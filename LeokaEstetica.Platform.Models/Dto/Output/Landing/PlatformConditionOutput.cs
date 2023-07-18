namespace LeokaEstetica.Platform.Models.Dto.Output.Landing;

/// <summary>
/// Класс выходной модели преимуществ платформы.
/// </summary>
public class PlatformConditionOutput
{
    /// <summary>
    /// Заголовок.
    /// </summary>
    public string PlatformConditionTitle { get; set; }

    /// <summary>
    /// Название преимущества платформы.
    /// </summary>
    public string PlatformConditionSubTitle { get; set; }

    /// <summary>
    /// Позиция.
    /// </summary>
    public int Position { get; set; }
}