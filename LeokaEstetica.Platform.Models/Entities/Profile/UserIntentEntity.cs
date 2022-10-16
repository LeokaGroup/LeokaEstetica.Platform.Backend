namespace LeokaEstetica.Platform.Models.Entities.Profile;

/// <summary>
/// Класс сопоставляется с таблицей целей на платформе, которые выбрал пользователь Profile.UserIntents.
/// </summary>
public class UserIntentEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long UserIntentId { get; set; }

    /// <summary>
    /// Id цели.
    /// </summary>
    public int IntentId { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Позиция.
    /// </summary>
    public int Position { get; set; }
}