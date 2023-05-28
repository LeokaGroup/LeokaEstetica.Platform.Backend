namespace LeokaEstetica.Platform.Models.Entities.Profile;

/// <summary>
/// Класс сопоставляется с таблицей навыков пользователя.
/// </summary>
public class UserSkillEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int UserSkillId { get; set; }

    /// <summary>
    /// Id навыка из таблицы общих навыков.
    /// </summary>
    public int SkillId { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Позиция.
    /// </summary>
    public int Position { get; set; }
}