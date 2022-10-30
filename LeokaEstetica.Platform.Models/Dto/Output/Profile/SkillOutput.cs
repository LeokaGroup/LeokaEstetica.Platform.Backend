namespace LeokaEstetica.Platform.Models.Dto.Output.Profile;

/// <summary>
/// Класс выходной модели для списка навыков профиля пользователя.
/// </summary>
public class SkillOutput
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

    /// <summary>
    /// Выбран ли навык был ранее пользователем.
    /// </summary>
    public bool IsSelected { get; set; }
}