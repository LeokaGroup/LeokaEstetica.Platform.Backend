namespace LeokaEstetica.Platform.Models.Entities.Landing;

/// <summary>
/// Класс сопоставляется с таблицей преимуществ платформы dbo.PlatformConditions.
/// </summary>
public class PlatformConditionEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int PlatformConditionId { get; set; }

    /// <summary>
    /// Заголовок.
    /// </summary>
    public string? PlatformConditionTitle { get; set; }

    /// <summary>
    /// Название преимущества платформы.
    /// </summary>
    public string? PlatformConditionSubTitle { get; set; }

    /// <summary>
    /// Позиция.
    /// </summary>
    public int Position { get; set; }
}