using LeokaEstetica.Platform.Models.Entities.Moderation;

namespace LeokaEstetica.Platform.Database.Abstractions.Moderation.Resume;

/// <summary>
/// Абстракция репозитория модерации анкет пользователей.
/// </summary>
public interface IResumeModerationRepository
{
    /// <summary>
    /// Метод получает список анкет для модерации.
    /// </summary>
    /// <returns>Список анкет.</returns>
    Task<IEnumerable<ModerationResumeEntity>> ResumesModerationAsync();
}