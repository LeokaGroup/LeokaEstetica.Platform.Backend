using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Models.Entities.User;

namespace LeokaEstetica.Platform.Models.Entities.Moderation;

/// <summary>
/// Класс сопоставляется с таблицей Moderation.ProfileRemarks.
/// </summary>
public class ResumeRemarkEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long RemarkId { get; set; }

    /// <summary>
    /// Id анкеты.
    /// </summary>
    public long ProfileInfoId { get; set; }

    /// <summary>
    /// Название поля.
    /// </summary>
    public string FieldName { get; set; }

    /// <summary>
    /// Текст замечания.
    /// </summary>
    public string RemarkText { get; set; }

    /// <summary>
    /// Русское название замечания.
    /// </summary>
    public string RussianName { get; set; }

    /// <summary>
    /// Id модератора.
    /// </summary>
    public long ModerationUserId { get; set; }

    /// <summary>
    /// FK на пользователя.
    /// </summary>
    public UserEntity ModerationUser { get; set; }

    /// <summary>
    /// Дата создания замечания.
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Id статуса замечания.
    /// </summary>
    public int RemarkStatusId { get; set; }

    /// <summary>
    /// FK на статус замечания.
    /// </summary>
    public RemarkStatuseEntity RemarkStatuse { get; set; }

    /// <summary>
    /// FK на анкету пользователя.
    /// </summary>
    public ProfileInfoEntity ProfileInfo { get; set; }
}