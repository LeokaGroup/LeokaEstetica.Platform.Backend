using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Output.Profile;

/// <summary>
/// Класс выходной модели сохранения списка навыков пользователя в анкете.
/// </summary>
public class SaveUserSkillOutput : FrontErrorOutput
{
    /// <summary>
    /// Список навыков.
    /// </summary>
    public List<SkillOutput> ProfileSkills { get; set; }

    /// <summary>
    /// Успешно ли сохранение.
    /// </summary>
    public bool IsSuccess { get; set; }
}