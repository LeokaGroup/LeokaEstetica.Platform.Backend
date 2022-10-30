using LeokaEstetica.Platform.Models.Dto.Output.Profile;

namespace LeokaEstetica.Platform.Models.Dto.Input.Profile;

/// <summary>
/// Класс входной модели для сохранения выбранных пользователем навыков.
/// </summary>
public class SaveSkillInput
{
    /// <summary>
    /// Список выбранных навыков пользователя.
    /// </summary>
    public IEnumerable<SkillOutput> SelectedSkills { get; set; }
}