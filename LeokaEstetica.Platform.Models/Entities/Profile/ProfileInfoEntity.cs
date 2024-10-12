using LeokaEstetica.Platform.Models.Entities.Moderation;

namespace LeokaEstetica.Platform.Models.Entities.Profile;

/// <summary>
/// Класс сопоставляется с таблицей информации о пользователе (Обо мне) Profile.ProfilesInfo.
/// </summary>
public class ProfileInfoEntity
{
    public ProfileInfoEntity()
    {
        ModerationResumes = new HashSet<ModerationResumeEntity>();
    }

    /// <summary>
    /// PK.
    /// </summary>
    public long ProfileInfoId { get; set; }

    /// <summary>
    /// Фамилия.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Имя.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Отчество.
    /// </summary>
    public string? Patronymic { get; set; }

    /// <summary>
    /// Отображать ли первую букву фамилии.
    /// </summary>
    public bool IsShortFirstName { get; set; }

    /// <summary>
    /// Ссылка на телегу либо ник.
    /// </summary>
    public string? Telegram { get; set; }

    /// <summary>
    /// Ватсап номер телефона.
    /// </summary>
    public string? WhatsApp { get; set; }

    /// <summary>
    /// Ссылка на ВК либо ник.
    /// </summary>
    public string? Vkontakte { get; set; }

    /// <summary>
    /// Ссылка на другую соц.сеть.
    /// </summary>
    public string? OtherLink { get; set; }

    /// <summary>
    /// Обо мне.
    /// </summary>
    public string? Aboutme { get; set; }

    /// <summary>
    /// Должность.
    /// </summary>
    public string? Job { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Список анкет.
    /// </summary>
    public ICollection<ModerationResumeEntity> ModerationResumes { get; set; }

    /// <summary>
    /// Опыт работы.
    /// </summary>
    public string? WorkExperience { get; set; }

    /// <summary>
    /// Список замечаний анкет.
    /// </summary>
    public ICollection<ResumeRemarkEntity> ResumeRemarks { get; set; }
}