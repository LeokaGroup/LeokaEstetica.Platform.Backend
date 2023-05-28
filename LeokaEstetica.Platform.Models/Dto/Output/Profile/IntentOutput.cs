namespace LeokaEstetica.Platform.Models.Dto.Output.Profile;

/// <summary>
/// Класс выходной модели для списка целей на платформе для профиля пользователя.
/// </summary>
public class IntentOutput
{
    /// <summary>
    /// PK.
    /// </summary>
    public int IntentId { get; set; }

    /// <summary>
    /// Название цели.
    /// </summary>
    public string IntentName { get; set; }

    /// <summary>
    /// Системное название цели.
    /// </summary>
    public string IntentSysName { get; set; }

    /// <summary>
    /// Позиция.
    /// </summary>
    public int Position { get; set; }
}