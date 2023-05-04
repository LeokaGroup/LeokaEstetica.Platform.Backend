using LeokaEstetica.Platform.Models.Dto.Output.Profile;

namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Resume;

/// <summary>
/// Класс композитной модели для сбора данных для профиля пользователя. 
/// </summary>
public class ProfileInfoCompositeOutput
{
    /// <summary>
    /// Данные профиля.
    /// </summary>
    public ProfileInfoOutput ProfileInfo { get; set; }

    /// <summary>
    /// Список целей.
    /// </summary>
    public IEnumerable<IntentOutput> Intents { get; set; }

    /// <summary>
    /// Список навыков.
    /// </summary>
    public IEnumerable<SkillOutput> Skills { get; set; }
}