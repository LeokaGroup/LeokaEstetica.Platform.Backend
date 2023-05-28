namespace LeokaEstetica.Platform.Models.Entities.Profile;

/// <summary>
/// Класс сопоставляется с таблицей целей на платформе, которые может выбрать пользователь Profile.Intents.
/// </summary>
public class IntentEntity
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