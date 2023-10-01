namespace LeokaEstetica.Platform.Models.Entities.Profile;

/// <summary>
/// Класс сопоставляется с таблицей навыков пользователей Profile.Skills.
/// </summary>
public class SkillEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int SkillId { get; set; }

    /// <summary>
    /// Название навыка.
    /// </summary>
    public string SkillName { get; set; }

    /// <summary>
    /// Системное название навыка.
    /// </summary>
    public string SkillSysName { get; set; }

    /// <summary>
    /// Позиция.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Тэг навыка.
    /// </summary>
    public string Tag { get; set; }
}