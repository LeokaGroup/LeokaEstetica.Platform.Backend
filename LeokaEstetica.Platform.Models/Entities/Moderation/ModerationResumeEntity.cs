using LeokaEstetica.Platform.Models.Entities.Profile;

namespace LeokaEstetica.Platform.Models.Entities.Moderation;

/// <summary>
/// Класс сопоставляется
/// </summary>
public class ModerationResumeEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long ModerationId { get; set; }

    /// <summary>
    /// Id статуса момодерации.
    /// </summary>
    public int ModerationStatusId { get; set; }
    
    /// <summary>
    /// Дата модерации.
    /// </summary>
    public DateTime DateModeration { get; set; }

    /// <summary>
    /// Id анкеты пользователя.
    /// </summary>
    public long ProfileInfoId { get; set; }

    /// <summary>
    /// FK на статусы модерации.
    /// </summary>
    public ModerationStatusEntity ModerationStatus { get; set; }

    /// <summary>
    /// FK на анкеты.
    /// </summary>
    public ProfileInfoEntity ProfileInfo { get; set; }
}